using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseScreen : MonoBehaviour
{
    [Header("Screen Elements")]
    [SerializeField] private CanvasGroup _graphics;

    [Header("Animation Timings")] 
    [SerializeField] private float _graphicsFadeTime = 0.21f;
    [SerializeField] private LeanTweenType _graphicsTweenType = LeanTweenType.easeOutCubic;


    public virtual void EnterScreen() => FadeGraphics(0,1);
    public virtual void ExitScreen() => FadeGraphics(1,0);

    private void FadeGraphics(float from, float to)
    {
        FadeElement(
            _graphics.gameObject,
            from,
            to,
            _graphicsFadeTime,
            _graphicsTweenType,
            (value) => _graphics.alpha = value
        );
    }
    
    private void FadeElement(GameObject element, float from, float to, float duration, LeanTweenType easeType, Action<float> callback)
    {
        LeanTween.value(element, from, to, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnUpdate(callback);
    }
}
