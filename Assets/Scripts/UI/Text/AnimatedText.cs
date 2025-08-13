using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = .2f;
    [SerializeField] private LeanTweenType _curve = LeanTweenType.easeInOutCubic;

    private int _alphaTweenId = -1;

    public UniTask TweenTextVisibility(float to, CancellationToken ct)
    {
        if (_text == null) return UniTask.CompletedTask;

        float from = _text.color.a;

        if (_alphaTweenId >= 0) { LeanTween.cancel(_alphaTweenId); _alphaTweenId = -1; }

        var completionSource = new UniTaskCompletionSource();

        _alphaTweenId = LeanTween.value(gameObject, from, to, _duration)
            .setEase(_curve)
            .setIgnoreTimeScale(true)
            .setOnUpdate((a) =>
            {
                var c = _text.color; c.a = a; _text.color = c;
            })
            .setOnComplete(() =>
            {
                _alphaTweenId = -1;
                completionSource.TrySetResult();
            }).id;

        ct.Register(() =>
        {
            if (_alphaTweenId >= 0)
            {
                LeanTween.cancel(_alphaTweenId);
                _alphaTweenId = -1;
                completionSource.TrySetCanceled();
            }
        });

        return completionSource.Task;
    }
}