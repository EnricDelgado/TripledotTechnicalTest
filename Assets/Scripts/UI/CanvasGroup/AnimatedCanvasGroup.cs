using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimatedCanvasGroup : TweenedElement
{
    [SerializeField] private CanvasGroup _canvasGroup;
    
    public override async UniTask PlayClip(CanvasTweenClip clip, CancellationToken cancelToken)
    {
        List<UniTask> tasks = new();
        
        if (clip.useCanvasGroupAlpha)
        {
            tasks.Add(TweenCanvasGroupVisibility(clip.canvasAlphaFrom, clip.canvasAlphaTo, clip.canvasAlphaDuration, clip.canvasAlphaEase, cancelToken));
        }
        
        await UniTask.WhenAll(tasks);
    }
    
    private UniTask TweenCanvasGroupVisibility(float from, float to, float duration, LeanTweenType ease, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.CanvasGroup);
        var completionSource = new UniTaskCompletionSource();

        int actionID = LeanTween.value(gameObject, from, to, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate((value) => _canvasGroup.alpha = value)
            .setOnComplete(() =>
                {
                    ClearChannel(TweenChannel.CanvasGroup);
                    completionSource.TrySetResult();
                }
            )
            .id;
        
        FillChannel(TweenChannel.CanvasGroup, actionID);
        RegisterChannelCancellationToken(cancelToken, TweenChannel.CanvasGroup, actionID, completionSource);

        return completionSource.Task;
    }
}