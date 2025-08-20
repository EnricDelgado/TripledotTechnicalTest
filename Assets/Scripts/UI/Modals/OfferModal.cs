using System;
using UnityEngine;
using UnityEngine.UI;

public class OfferModal : BaseModal
{
    [Header("Offer Elements")]
    [SerializeField] private RectTransform _textA;
    [SerializeField] private RectTransform _textB;
    [SerializeField] private RectTransform _textHolder;
    [SerializeField] private RectTransform _shineOverlayA;
    [SerializeField] private RectTransform _shineOverlayB;
    
    [Header("Offer Animation Timings")]
    [Header("Scale Animation")]
    [SerializeField] private float _scaleDuration = 0.6f;
    [SerializeField] private LeanTweenType _scaleEase = LeanTweenType.easeOutBack;
    
    [Header("Move Animation")]
    [SerializeField] private float _moveDuration = 1.0f;
    [SerializeField] private float _yOffset = 300f;
    [SerializeField] private LeanTweenType _moveEase = LeanTweenType.easeInOutBack;
    
    [Header("Shine Animation")]
    [SerializeField] private float _shineDuration = .47f;
    [SerializeField] private float _shineOffset = 100f;
    [SerializeField] private LeanTweenType _shineEase = LeanTweenType.easeInOutSine;
    
    private Vector2 _textHolderInitialPosition;
    private Vector2 _shineAOverlayInitialPosition;
    private Vector2 _shineBOverlayInitialPosition;


    private void Awake()
    {
        _textHolderInitialPosition = _textHolder.anchoredPosition;
        _shineAOverlayInitialPosition = _shineOverlayA.anchoredPosition;
        _shineBOverlayInitialPosition = _shineOverlayB.anchoredPosition;
    }

    public override void ShowModal()
    {
        base.ShowModal();
        AnimateTexts();
    }

    public override void HideModal()
    {
        base.HideModal();
        
        _textA.localScale = Vector3.zero;
        _textB.localScale = Vector3.zero;
        
        ResetRect(_textHolder, _textHolderInitialPosition);
        ResetRect(_shineOverlayA, _shineAOverlayInitialPosition);
        ResetRect(_shineOverlayB, _shineBOverlayInitialPosition);
    }

    void AnimateTexts()
    {
        LeanTween.scale(_textA, Vector3.one, _scaleDuration)
            .setEase(_scaleEase)
            .setOnComplete(() =>
            {
                ShineSweep(_shineOverlayA, () =>
                {
                    LeanTween.scale(_textB, Vector3.one, _scaleDuration)
                        .setEase(_scaleEase)
                        .setOnComplete(() =>
                        {
                            ShineSweep(_shineOverlayB, () =>
                            {
                                LeanTween.moveLocalY(_textHolder.gameObject, _textHolder.localPosition.y + _yOffset, _moveDuration)
                                    .setEase(_moveEase);
                            });
                        });
                });
            });
    }

    void ShineSweep(RectTransform shine, Action onComplete)
    {
        LeanTween.moveX(shine, _shineOffset, _shineDuration)
            .setEase(_shineEase)
            .setOnComplete(() => onComplete?.Invoke());
    }

    private void ResetRect(RectTransform transform, Vector2 position) => transform.anchoredPosition = position;
}