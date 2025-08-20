using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimatedPopup : BasePopup
{
    [Header("Animated Popup")]
    [Header("Animated Clip Elements")]
    // [SerializeField] private AnimatedCanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private AnimatedIcon _popup;
    
    [Header("Animation Settings")]
    // [SerializeField]  private TweenClip _canvasGroupFadeInClip;
    // [SerializeField]  private TweenClip _canvasGroupFadeOutClip;
    [SerializeField] private TweenClip _jellyBounceClip;
    [SerializeField] private LeanTweenType _popupFadeEase = LeanTweenType.easeInOutCubic;
    [SerializeField] private float _popupFadeTime = .32f;
    
    
    protected override void ShowPopup()
    {
        // _canvasGroup.PlayClip(_canvasGroupFadeInClip, CancellationToken.None).Forget();
        
        FadeElementIn(
            _canvasGroup.gameObject,
            _popupFadeTime,
            _popupFadeEase,
            (value) =>
            {
                _canvasGroup.alpha = value;
            }
        ).Forget();
        
        _popup.PlayClip(_jellyBounceClip, CancellationToken.None).Forget();
    }

    protected override void HidePopup()
    {
        // _canvasGroup.PlayClip(_canvasGroupFadeOutClip, CancellationToken.None).Forget();
        FadeElementOut(
            _canvasGroup.gameObject,
            _popupFadeTime,
            _popupFadeEase,
            (value) =>
            {
                _canvasGroup.alpha = value;
            }
        ).Forget();
    }

    private async UniTaskVoid FadeElementIn(GameObject element, float duration, LeanTweenType ease, Action<float> updateCallback)
    {
        await FadeElement(
            element, 
            0,
            1,
            duration,
            ease,
            updateCallback
        );
    }
    
    private async UniTaskVoid FadeElementOut(GameObject element, float duration, LeanTweenType ease, Action<float> updateCallback)
    {
        await FadeElement(
            element, 
            1,
            0,
            duration,
            ease,
            updateCallback
        );
    }
}