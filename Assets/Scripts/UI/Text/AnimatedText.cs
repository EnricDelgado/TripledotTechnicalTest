using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public class AnimatedText : TweenedElement
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = .2f;
    [SerializeField] private LeanTweenType _curve = LeanTweenType.easeInOutCubic;

    private int _alphaTweenId = -1;

    public UniTask TweenTextVisibility(float to, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.Text);
        var completionSource = new UniTaskCompletionSource();

        float from = _text.color.a;

        _alphaTweenId = LeanTween.value(gameObject, from, to, _duration)
            .setEase(_curve)
            .setIgnoreTimeScale(true)
            .setOnUpdate((a) =>
            {
                var c = _text.color; c.a = a; _text.color = c;
            })
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.Text);
                completionSource.TrySetResult();
            }).id;
        
        FillChannel(TweenChannel.Text, _alphaTweenId);
        
        RegisterChannelCancellationToken(cancelToken, TweenChannel.Text, completionSource);

        return completionSource.Task;
    }
}