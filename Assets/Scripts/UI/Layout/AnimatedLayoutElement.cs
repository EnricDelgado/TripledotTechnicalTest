using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedLayoutElement : TweenedElement
{
    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;

    public LayoutElement Element => _layoutElement;

    
    private void Awake()
    {
        _layoutElement = GetComponent<LayoutElement>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public async override UniTask PlayClip(TweenClip clip, CancellationToken cancelToken)
    {
        var tasks = new List<UniTask>();

        if (clip.useMove) throw new NotImplementedException();

        if (clip.useScale)
        {
            if (clip.separateAxis)
            {
                bool widthAxis = (clip.effectAxis == EffectAxis.X);

                float from = widthAxis
                    ? (_layoutElement.preferredWidth  > 0 ? _layoutElement.preferredWidth  : _rectTransform.rect.width)
                    : (_layoutElement.preferredHeight > 0 ? _layoutElement.preferredHeight : _rectTransform.rect.height);

                float to = 0;

                switch (clip.scaleToMode)
                {
                    case ToModeScale.Absolute:
                        to = widthAxis ? clip.scaleTo.x : clip.scaleTo.y;
                        break;
                    case ToModeScale.ByOffset:
                        to = from + (widthAxis ? clip.scaleDelta.x : clip.scaleDelta.y);
                        break;
                    case ToModeScale.MultiplyBy:
                        to = from * clip.scaleMultiply;
                        break;
                }

                tasks.Add(TweenAxis(clip.effectAxis, from, to, clip.scaleDuration, clip.scaleEase, cancelToken));
            }
            else
            {
                Vector3 from = _rectTransform.localScale;
                Vector3 to = from;

                switch (clip.scaleToMode)
                {
                    case ToModeScale.Absolute:
                        to = clip.scaleTo;
                        break;
                    case ToModeScale.ByOffset:
                        to = from + clip.scaleDelta;
                        break;
                    case ToModeScale.MultiplyBy:
                        to = from * clip.scaleMultiply;
                        break;
                }

                tasks.Add(TweenScale(from, to, clip.scaleDuration, clip.scaleEase, cancelToken));
            }
        }

        if (clip.useAlpha) throw new NotImplementedException();

        await UniTask.WhenAll(tasks);
    }


    private UniTask TweenAxis(EffectAxis axis, float from, float to, float duration, LeanTweenType ease, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Scale);
        var completionSource = new UniTaskCompletionSource();
        
        int actionID = LeanTween.value(gameObject, from, to, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate((value) =>
            {
                if(axis == EffectAxis.X) 
                    _layoutElement.preferredWidth = value;
                else
                    _layoutElement.preferredHeight = value;
                
                if (transform.parent is RectTransform parent)
                    LayoutRebuilder.MarkLayoutForRebuild(parent);
            })
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
    
    private UniTask TweenScale(Vector3 from, Vector3 to, float duration, LeanTweenType ease, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Scale);
        var completionSource = new UniTaskCompletionSource();
        
        _rectTransform.localScale = from;
        
        int actionID = LeanTween.scale(gameObject, to, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.Scale);
                completionSource.TrySetResult();
            }).id;
        
        FillChannel(TweenChannel.Scale, actionID);

        RegisterChannelCancellationToken(cancelToken, TweenChannel.Scale, actionID, completionSource);
        
        return completionSource.Task;
    }
}
