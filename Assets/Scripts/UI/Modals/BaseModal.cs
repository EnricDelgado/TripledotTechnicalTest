using UnityEngine;

public class BaseModal : MonoBehaviour
{
    [Header("Base Modal")]
    [Header("Base Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    
    [Header("Base Animation Parameters")]
    [SerializeField] private float _fadeDuration;
    [SerializeField] private LeanTweenType _fadeEaseType = LeanTweenType.easeInBounce;
    

    public virtual void ShowModal()
    {
        FadeCanvas(0, 1, _fadeDuration);
        _canvasGroup.blocksRaycasts = true;
    }

    public virtual void HideModal()
    {
        FadeCanvas(1, 0, _fadeDuration);
        _canvasGroup.blocksRaycasts = false;
    }

    public void FadeCanvas(float from, float to, float duration)
    {
        LeanTween.value(gameObject, from, to, duration)
            .setEase(_fadeEaseType)
            .setIgnoreTimeScale(true)
            .setOnUpdate((value) => _canvasGroup.alpha = value);
    }
}
