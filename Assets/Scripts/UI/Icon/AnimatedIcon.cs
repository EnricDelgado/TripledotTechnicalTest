using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class AnimatedIcon : TweenedElement
{
    [Header("Icon Elements")]
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _iconTransform;
    
    [Header("Animation Settings")] 
    [Header("Position")] 
    [SerializeField] private float _movementDuration = .3f;
    [SerializeField] private LeanTweenType _movementEase = LeanTweenType.easeOutBack;
    
    [Header("Scale")] 
    [SerializeField] private float _resizeDuration = 0.3f;
    [SerializeField] private LeanTweenType _resizeCurve = LeanTweenType.easeInOutCubic;
    
    [Header("Color")] 
    [SerializeField] private float _alphaDuration = .3f;
    [SerializeField] private LeanTweenType _alphaEase = LeanTweenType.easeOutBack;

    private int _tweenId = -1;

    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private float _initialAlpha;
    
    public RectTransform RectTransform => _iconTransform;


    private void Awake()
    {
        if (_iconTransform == null) _iconTransform = GetComponent<RectTransform>();
        if (_icon == null) _icon = GetComponent<Image>();

        _initialScale = _iconTransform.localScale;
        _initialPosition = _iconTransform.localPosition;
        _initialAlpha = _icon.color.a;
    }

    private void OnDisable() => CancelAllChannels();
    
    #region Scale Tween Logic
    private UniTask TweenIconScaleFrom(Vector3 from, Vector3 to, float duration, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Scale);
        var completionSource = new UniTaskCompletionSource();

        _iconTransform.localScale = from;
        
        int actionID = LeanTween.scale(_iconTransform, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.Scale);
                completionSource.TrySetResult();
            })
            .id;
        
        FillChannel(TweenChannel.Scale, actionID);
        
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Scale, completionSource);
        
        return completionSource.Task;
    }
    
    public async UniTask TweenIconScale(Vector3 to, CancellationToken cancelToken) => await TweenIconScaleFrom(_iconTransform.localScale, to, _resizeDuration, _resizeCurve, cancelToken);
    public async UniTaskVoid ResetIconScale(CancellationToken cancellationToken) => await TweenIconScale(_initialScale, cancellationToken);
    #endregion
    
    #region Movement Tween Logic
    private UniTask TweenIconPositionFrom(Vector3 from, Vector3 to, float duration, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Position);
        
        var completionSource = new UniTaskCompletionSource();
        
        _iconTransform.localPosition = from;

        int actionID = LeanTween.moveLocal(_iconTransform.gameObject, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.Position);
                completionSource.TrySetResult();
            })
            .id;
        
        FillChannel(TweenChannel.Position, actionID);
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Position, completionSource);
        
        return completionSource.Task;
    }
    
    public async UniTask TweenIconPosition(Vector3 to, CancellationToken cancelToken) => await TweenIconPositionFrom(_iconTransform.localPosition, to, _movementDuration, _movementEase, cancelToken);
    public async UniTask ResetIconPosition(CancellationToken cancelToken) => await TweenIconPosition(_initialPosition, cancelToken);
    #endregion
    
    #region Color Tween Logic
    private UniTask TweenIconAlphaFrom(float from, float to, float duration, LeanTweenType easeType,
        CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Alpha);

        var completionSource = new UniTaskCompletionSource();

        var color = _icon.color;
        color.a = from;
        _icon.color = color;

        int actionID = LeanTween.value(gameObject, from, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float a) =>
            {
                var col = _icon.color;
                col.a = a;
                _icon.color = col;
            })
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.Alpha);
                completionSource.TrySetResult();
            }).id;
        
        FillChannel(TweenChannel.Alpha, actionID);
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Alpha, completionSource);
        
        return completionSource.Task;
    }
    
    public async UniTask TweenIconAlpha(float to, CancellationToken cancelToken) => await TweenIconAlphaFrom(_icon.color.a, to, _alphaDuration, _alphaEase, cancelToken);
    public async UniTask ResetIconAlpha(CancellationToken cancelToken) => await TweenIconAlpha(_initialAlpha, cancelToken);
    #endregion
    
    #region Clip Tween Logic
    public async UniTask PlayClip(IconAnimClip clip, CancellationToken ct)
    {
        var tasks = new List<UniTask>(3);

        if (clip.useScale)
            tasks.Add(TweenIconScaleFrom(
                from: clip.scaleFrom,
                to:   clip.scaleTo,
                duration: clip.scaleDuration,
                easeType: clip.scaleEase,
                cancelToken: ct
            ));

        if (clip.useMove)
            tasks.Add(TweenIconPositionFrom(
                from: clip.moveFrom,
                to:   clip.moveTo,
                duration: clip.moveDuration,
                easeType: clip.moveEase,
                cancelToken: ct
            ));

        if (clip.useAlpha)
            tasks.Add(TweenIconAlphaFrom(
                from: clip.alphaFrom,
                to:   clip.alphaTo,
                duration: clip.alphaDuration,
                easeType: clip.alphaEase,
                cancelToken: ct
            ));

        await UniTask.WhenAll(tasks);
    }
    #endregion
}