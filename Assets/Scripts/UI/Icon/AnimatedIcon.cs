using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class AnimatedIcon : TweenedElement
{
    [Header("Icon Elements")]
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _iconTransform;
    
    private Vector3 _initialPosition;
    private Vector3 _initialScale;


    private void Awake()
    {
        if (_iconTransform == null) _iconTransform = GetComponent<RectTransform>();
        if (_icon == null) _icon = GetComponent<Image>();

        _initialScale = _iconTransform.localScale;
        _initialPosition = _iconTransform.localPosition;
    }
    
    public override async UniTask PlayClip(TweenClip clip, CancellationToken cancelToken)
    {
        var tasks = new List<UniTask>(3);

        if (clip.useScale)
        {
            var from = (clip.scaleFrom == FromMode.Current) ? _iconTransform.localScale : _initialScale;
            Vector3 to;
            
            switch (clip.scaleToMode)
            {
                case ToModeScale.Absolute:   
                    to = clip.scaleTo; 
                    break;
                case ToModeScale.MultiplyBy: 
                    to = from * clip.scaleMultiply; 
                    break;
                case ToModeScale.ByOffset:   
                    to = from + clip.scaleDelta; 
                    break;
                default:                     
                    to = from; 
                    break;
            }

            tasks.Add(TweenIconScaleFrom(from, to, clip.scaleDuration, clip.scaleEase, cancelToken));
        }

        if (clip.useMove)
        {
            var from = (clip.moveFrom == FromMode.Current) ? _iconTransform.localPosition : _initialPosition;
            Vector3 to = (clip.moveToMode == ToModePosition.Absolute) ? clip.moveTo : from + clip.moveOffset;

            tasks.Add(TweenIconPositionFrom(from, to, clip.moveDuration, clip.moveEase, cancelToken));
        }

        if (clip.useAlpha)
        {
            float from = (clip.alphaFrom == FromMode.Current) ? _icon.color.a : clip.initalAlpha;

            tasks.Add(TweenIconAlphaFrom(from, clip.alphaTo, clip.alphaDuration, clip.alphaEase, cancelToken));
        }

        if (clip.useShake)
            tasks.Add(TweenShake(clip.shakeDuration, clip.shakeMagnitude, clip.shakeFrequency, clip.shakeEase, cancelToken));

        if (clip.useJellyBounce)
            tasks.Add(TweenJellyBounce(clip.jellyBounceDuration, clip.jellyOvershootScale, clip.jellyBounceEase, cancelToken));
        
        

        await UniTask.WhenAll(tasks);
    }
    
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
        
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Scale, actionID, completionSource);
        
        return completionSource.Task;
    }
    
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
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Position, actionID, completionSource);
        
        return completionSource.Task;
    }
    
    private UniTask TweenIconAlphaFrom(float from, float to, float duration, LeanTweenType easeType, CancellationToken cancelToken)
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
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Alpha, actionID, completionSource);
        
        return completionSource.Task;
    }

    private UniTask TweenShake(float duration, float magnitude, float frequency, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Shake);

        var completionSource = new UniTaskCompletionSource();

        Vector2 originalPos = _iconTransform.anchoredPosition;

        float seedX = Random.Range(0f, 999f);
        float seedY = seedX + 100.123f;

        int actionID = LeanTween.value(gameObject, 0f, 1f, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float t) =>
            {
                float time = Time.unscaledTime;
                float nX = Mathf.PerlinNoise(seedX, time * frequency) * 2f - 1f;
                float nY = Mathf.PerlinNoise(seedY, time * frequency) * 2f - 1f;

                float envelope = 1f - Mathf.Abs(2f * t - 1f);
                float amt = magnitude * (0.5f + 0.5f * envelope);

                _iconTransform.anchoredPosition = originalPos + new Vector2(nX, nY) * amt;
            })
            .setOnComplete(() =>
            {
                _iconTransform.anchoredPosition = originalPos;

                ClearChannel(TweenChannel.Shake);
                completionSource.TrySetResult();
            }).id;

        FillChannel(TweenChannel.Shake, actionID);
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Shake, actionID, completionSource);

        return completionSource.Task;
    }
    
    private UniTask TweenJellyBounce(float duration, float overshootScale, LeanTweenType easeType, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Jelly);

        var completionSource = new UniTaskCompletionSource();

        Vector3 originalScale = _iconTransform.localScale;
        
        float overshootRange = overshootScale * 0.33f; 
        float oversoot = Random.Range(overshootScale -= overshootRange, overshootScale += overshootRange);

        int actionID = LeanTween.value(gameObject, 0f, 1f, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float t) =>
            {
                float squash = Mathf.Sin(t * Mathf.PI);
                float stretch = Mathf.Cos(t * Mathf.PI);

                float scaleX = Mathf.Lerp(originalScale.x, originalScale.x * oversoot, squash);
                float scaleY = Mathf.Lerp(originalScale.y, originalScale.y / oversoot, stretch);

                _iconTransform.localScale = new Vector3(scaleX, scaleY, originalScale.z);
            })
            .setOnComplete(() =>
            {
                _iconTransform.localScale = originalScale;

                ClearChannel(TweenChannel.Jelly);
                completionSource.TrySetResult();
            }).id;

        FillChannel(TweenChannel.Jelly, actionID);
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Jelly, actionID, completionSource);

        return completionSource.Task;
    }
}