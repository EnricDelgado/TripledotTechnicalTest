using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedLayoutElement : MonoBehaviour
{
    [SerializeField] private float _resizeDuration = 0.18f;
    [SerializeField] private AnimationCurve _resizeCurve = AnimationCurve.EaseInOut(0,0,1,1);

    private RectTransform _rectTransform;
    private LayoutElement _layoutElement;
    private int _animationID;

    public LayoutElement Element => _layoutElement;
    public RectTransform RectTransform => _rectTransform;


    private void Awake()
    {
        _layoutElement = GetComponent<LayoutElement>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void TweenWidthAsync(float to, CancellationToken ct)
    {
        _animationID = LeanTween.value(gameObject, _rectTransform.rect.width, to, _resizeDuration)
            .setEase(_resizeCurve)
            .setIgnoreTimeScale(true)
            .setOnUpdate((value) =>
            {
                _layoutElement.preferredWidth = value;
                if (transform.parent is RectTransform parent) LayoutRebuilder.MarkLayoutForRebuild(parent);
            })
            .setOnComplete(() =>
            {
                _animationID = -1;
            }).id;

        // Cancel hook
        ct.Register(() =>
        {
            if (_animationID >= 0)
            {
                LeanTween.cancel(_animationID);
                _animationID = -1;
            }
        });
    }
}
