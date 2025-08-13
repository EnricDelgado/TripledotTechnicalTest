using System.Threading;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = .2f;
    [SerializeField] private LeanTweenType _curve = LeanTweenType.easeInOutCubic;
    
    private int _tweenId;

    public void TweenTextVisibility(float to, CancellationToken cancellationToken)
    {
        _tweenId = LeanTween.value(_text.color.a, to, _duration)
            .setOnUpdate((value) =>
            {
                Color color = _text.color;
                color.a = value;
                _text.color = color;
            })
            .setEase(_curve)
            .id;

        cancellationToken.Register(() =>
        {
            if (_tweenId >= 0)
            {
                LeanTween.cancel(_tweenId);
                _tweenId = -1;
            }
        });
    }
}
