using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedLayoutElement : TweenedElement
{
    [SerializeField] private float _resizeDuration = 0.18f;
    [SerializeField] private LeanTweenType _resizeCurve = LeanTweenType.easeInBounce;

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
    private int _widthActionID;

    public LayoutElement Element => _layoutElement;


    private void Awake()
    {
        _layoutElement = GetComponent<LayoutElement>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public UniTask TweenWidthAsync(float to, CancellationToken cancelToken)
    {
        CancelChannel(TweenChannel.LayoutWidth);
        var completionSource = new UniTaskCompletionSource();
        
        float from = _layoutElement.preferredWidth > 0f ? _layoutElement.preferredWidth : _rectTransform.rect.width;

        _widthActionID = LeanTween.value(gameObject, from, to, _resizeDuration)
            .setEase(_resizeCurve)
            .setIgnoreTimeScale(true)
            .setOnUpdate((value) =>
            {
                _layoutElement.preferredWidth = value;
                if (transform.parent is RectTransform parent)
                    LayoutRebuilder.MarkLayoutForRebuild(parent);
            })
            .setOnComplete(() =>
            {
                ClearChannel(TweenChannel.LayoutWidth);
                completionSource.TrySetResult();
            }).id;

        RegisterChannelCancellationToken(cancelToken, TweenChannel.LayoutWidth, completionSource);
        
        return completionSource.Task;
    }
}
