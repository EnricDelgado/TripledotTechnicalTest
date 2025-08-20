using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnimatedText : TweenedElement
{
    // this could be cached on awake + required component and removed from here
    [SerializeField] private TextMeshProUGUI _text;

    
    public override async UniTask PlayClip(TweenClip clip, CancellationToken cancelToken)
    {
        var tasks = new List<UniTask>();

        if (clip.useMove)
        {
            throw new NotImplementedException();
        }

        if (clip.useScale)
        {
            throw new NotImplementedException();
        }

        if (clip.useAlpha)
        {
            float from = (clip.alphaFrom == FromMode.Current) ? _text.color.a : clip.initalAlpha;
            tasks.Add(TweenTextAlpha(from, clip.alphaTo, clip.alphaDuration, clip.alphaEase, cancelToken));
        }
        
        await UniTask.WhenAll(tasks);
    }

    public UniTask TweenTextAlpha(float from, float to, float duration, LeanTweenType ease, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Alpha);
        var completionSource = new UniTaskCompletionSource();
        
        int actionID = LeanTween.value(gameObject, from, to, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate((a) =>
            {
                var c = _text.color; c.a = a; _text.color = c;
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
}