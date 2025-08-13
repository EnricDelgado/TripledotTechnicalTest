using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedLayoutElement : MonoBehaviour
{
    [SerializeField] private float _resizeDuration = 0.18f;
    [SerializeField] private LeanTweenType _resizeCurve = LeanTweenType.easeInBounce;

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
    private int _widthActionID;

    public LayoutElement Element => _layoutElement;
    public RectTransform RectTransform => _rectTransform;


    private void Awake()
    {
        _layoutElement = GetComponent<LayoutElement>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public UniTask TweenWidthAsync(float to, CancellationToken ct)
    {
        float from = _layoutElement.preferredWidth > 0f ? _layoutElement.preferredWidth : _rectTransform.rect.width;

        if (_widthActionID >= 0) { LeanTween.cancel(_widthActionID); _widthActionID = -1; }

        var completionSource = new UniTaskCompletionSource();

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
                _widthActionID = -1;
                completionSource.TrySetResult();
            }).id;

        ct.Register(() =>
        {
            if (_widthActionID >= 0)
            {
                LeanTween.cancel(_widthActionID);
                _widthActionID = -1;
                completionSource.TrySetCanceled();
            }
        });

        return completionSource.Task;
    }
}
