using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class AnimatedIcon : MonoBehaviour
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
    
    private Dictionary<AnimationChannel, int> _tweens = new ();
    
    public RectTransform RectTransform => _iconTransform;
    public enum AnimationChannel {Scale, Position, Alpha}


    private void Awake()
    {
        if (_iconTransform == null) _iconTransform = GetComponent<RectTransform>();
        if (_icon == null) _icon = GetComponent<Image>();

        _initialScale = _iconTransform.localScale;
        _initialPosition   = _iconTransform.localPosition;
        _initialAlpha = _icon.color.a;
    }

    private void Start()
    {
        _initialPosition = _iconTransform.localPosition;
        _initialScale = _iconTransform.localScale;
    }

    private void OnDisable() => CancelAllChannels();

    #region Channel Logic
    private void FillChannel(AnimationChannel channel, int id) => _tweens[channel] = id;
    private void ClearChannel(AnimationChannel channel) => _tweens[channel] = -1;

    private void CancelChannel(AnimationChannel channel)
    {
        if (_tweens.TryGetValue(channel, out var id) && id >= 0)
        {
            LeanTween.cancel(id);
            ClearChannel(channel);
        }
    }

    private void CancelAllChannels()
    {
        LeanTween.cancelAll();
        _tweens.Clear();
    }

    private void RegisterChannelCancelationToken(CancellationToken token, AnimationChannel channel, UniTaskCompletionSource completionSource)
    {
        token.Register(() =>
        {
            CancelChannel(channel);
            completionSource.TrySetCanceled();
        });
    }
    #endregion
    
    #region Scale Tween Logic
    private UniTask TweenIconScaleFrom(Vector3 from, Vector3 to, float duration, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(AnimationChannel.Scale);
        var completionSoruce = new UniTaskCompletionSource();

        _iconTransform.localScale = from;
        
        int actionID = LeanTween.scale(_iconTransform, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                ClearChannel(AnimationChannel.Scale);
                completionSoruce.TrySetResult();
            })
            .id;
        
        FillChannel(AnimationChannel.Scale, actionID);
        
        RegisterChannelCancelationToken(cancelToken, AnimationChannel.Scale, completionSoruce);
        
        return completionSoruce.Task;
    }
    
    public async UniTask TweenIconScale(Vector3 to, CancellationToken cancelToken) => await TweenIconScaleFrom(_iconTransform.localScale, to, _resizeDuration, _resizeCurve, cancelToken);
    public async UniTaskVoid ResetIconScale(CancellationToken cancellationToken) => await TweenIconScale(_initialScale, cancellationToken);
    #endregion
    
    #region Movement Tween Logic
    private UniTask TweenIconPositionFrom(Vector3 from, Vector3 to, float duration, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(AnimationChannel.Position);
        
        var completionSoruce = new UniTaskCompletionSource();
        
        _iconTransform.localPosition = from;

        int actionID = LeanTween.moveLocal(_iconTransform.gameObject, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                ClearChannel(AnimationChannel.Position);
                completionSoruce.TrySetResult();
            })
            .id;
        
        FillChannel(AnimationChannel.Position, actionID);
        RegisterChannelCancelationToken(cancelToken, AnimationChannel.Position, completionSoruce);
        
        return completionSoruce.Task;
    }
    
    public async UniTask TweenIconPosition(Vector3 to, CancellationToken cancelToken) => await TweenIconPositionFrom(_iconTransform.localPosition, to, _movementDuration, _movementEase, cancelToken);
    public async UniTask ResetIconPosition(CancellationToken cancelToken) => await TweenIconPosition(_initialPosition, cancelToken);
    #endregion
    
    #region Color Tween Logic
    private UniTask TweenIconAlphaFrom(float from, float to, float duration, LeanTweenType easeType,
        CancellationToken cancelToken)
    {
        CancelChannel(AnimationChannel.Alpha);

        var completionSoruce = new UniTaskCompletionSource();

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
                ClearChannel(AnimationChannel.Alpha);
                completionSoruce.TrySetResult();
            }).id;
        
        FillChannel(AnimationChannel.Alpha, actionID);
        RegisterChannelCancelationToken(cancelToken, AnimationChannel.Alpha, completionSoruce);
        
        return completionSoruce.Task;
    }
    
    public async UniTask TweenIconAlpha(float to, CancellationToken cancelToken) => await TweenIconAlphaFrom(_icon.color.a, to, _alphaDuration, _alphaEase, cancelToken);
    public async UniTask ResetIconAlpha(CancellationToken cancelToken) => await TweenIconAlpha(_initialAlpha, cancelToken);
    #endregion
}