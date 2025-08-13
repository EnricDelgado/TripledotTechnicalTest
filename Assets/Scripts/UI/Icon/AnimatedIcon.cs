using System;
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
    
    [Header("Scale")] 
    [SerializeField] private float _resizeDuration = 0.3f;
    [SerializeField] private LeanTweenType _resizeCurve = LeanTweenType.easeInOutCubic;
    
    [Header("Color")] 
    [SerializeField] private float _alphaDuration = .3f;

    private int _tweenId = -1;

    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    
    public RectTransform RectTransform => _iconTransform;


    private void Start()
    {
        _originalPosition = _iconTransform.localPosition;
        _originalScale = _iconTransform.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TweenIconScaleAsync(_iconTransform.localScale + Vector3.one*2, new CancellationTokenSource().Token).Forget();
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
            ResetIconScaleAsync(new CancellationTokenSource().Token).Forget();
        
        if(Input.GetKeyDown(KeyCode.Alpha3))
            TweenIconMoveAsync(_iconTransform.localPosition + new Vector3(0, 20, 0), new CancellationTokenSource().Token).Forget();
        
        if(Input.GetKeyDown(KeyCode.Alpha4))
            ResetIconPositionAsync(new CancellationTokenSource().Token).Forget();
        
        if(Input.GetKeyDown(KeyCode.Alpha5))
            TweenIconAlphaAsync(0, _alphaDuration, new CancellationTokenSource().Token).Forget();
        
        if(Input.GetKeyDown(KeyCode.Alpha6))
            TweenIconAlphaAsync(1, _alphaDuration, new CancellationTokenSource().Token).Forget();
    }

    public UniTask TweenIconScaleAsync(Vector3 to, CancellationToken cancellationToken)
    {
        var completionSource = new UniTaskCompletionSource();

        _tweenId = LeanTween.scale(_iconTransform, to, _resizeDuration)
            .setEase(_resizeCurve)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                
                _tweenId = -1;
                completionSource.TrySetResult();
            }).id;

        _iconTransform.localScale = to;

        cancellationToken.Register(() =>
        {
            if (_tweenId >= 0)
            {
                LeanTween.cancel(_tweenId);
                _tweenId = -1;
            }
            completionSource.TrySetCanceled();
        });

        return completionSource.Task;
    }

    public async UniTaskVoid ResetIconScaleAsync(CancellationToken cancellationToken) => await TweenIconScaleAsync(_originalScale, cancellationToken);
    
    public UniTask TweenIconMoveAsync(Vector3 to, CancellationToken cancellationToken)
    {
        if (_iconTransform == null) return UniTask.CompletedTask;

        var completionSource = new UniTaskCompletionSource();

        _tweenId = LeanTween.moveLocal(_iconTransform.gameObject, to, _resizeDuration)
            .setEase(_resizeCurve)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                _tweenId = -1;
                completionSource.TrySetResult();
            }).id;
        
        _iconTransform.localPosition = to;

        cancellationToken.Register(() =>
        {
            if (_tweenId >= 0)
            {
                LeanTween.cancel(_tweenId);
                _tweenId = -1;
            }
            completionSource.TrySetCanceled();
        });

        return completionSource.Task;
    }
    
    public UniTask ResetIconPositionAsync(CancellationToken cancellationToken) => TweenIconMoveAsync(_originalPosition, cancellationToken);

    public UniTask TweenIconAlphaAsync(float to, float duration, CancellationToken cancellationToken)
    {
        var completionSource = new UniTaskCompletionSource();
        
        _tweenId = LeanTween.alpha(_icon.gameObject, to, duration)
            .setOnUpdate((float val) =>
                {
                    Color c = _icon.color;
                    c.a = val;
                    _icon.color = c;
                }
            )
            .setEase(_resizeCurve)
            .id;

        cancellationToken.Register(() =>
        {
            if (_tweenId >= 0)
            {
                LeanTween.cancel(_tweenId);
                _tweenId = -1;
            }
            completionSource.TrySetCanceled();
        });
        
        return completionSource.Task;
    }

}