using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndgameCounter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _recordBadge;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _startCount = 0;
    [SerializeField] private float _endCount = 100;
    [SerializeField] private int _currentRecord = 0;
    
    [Header("Animation Parameters")]
    [SerializeField] private float _counterDelay = 0.6f;
    [SerializeField] private float _recordDelay = 0.6f;
    [SerializeField] private float _counterDuration = .47f;
    [SerializeField] private float _iconFadeDuration = .18f;
    [SerializeField] private float _badgeFadeDuration = .18f;
    [SerializeField] private LeanTweenType _textEase = LeanTweenType.easeInCubic;
    [SerializeField] private LeanTweenType _iconEase = LeanTweenType.easeOutCubic;
    [SerializeField] private LeanTweenType _badgeEase = LeanTweenType.easeOutCubic;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem _iconVFX;
    [SerializeField] private ParticleSystem _recordBadgeVFX;

    
    public async UniTask Enter()
    {
        ShowIcon(_icon, _iconFadeDuration, _iconEase).Forget();
        PlayPS(_iconVFX).Forget();
        
        await UniTask.Delay(TimeSpan.FromSeconds(_counterDelay));
        TweenValue(
            _text.gameObject,
            0,
            1,
            .18f,
            _textEase,
            (value) =>
            {
                var color = _text.color;
                color.a = value;
                _text.color = color;
            }
        ).Forget();
        
        await TweenValue(
            _text.gameObject,
            _startCount,
            _endCount,
            _counterDuration,
            _textEase,
            (value) =>
            {
                var val = Mathf.FloorToInt(value);
                var newText = val.ToString();
                _text.SetText(newText);
            }
        );

        int newRecord = Int32.Parse(_text.text);
        
        if (newRecord > _currentRecord)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_recordDelay));
            ShowIcon(_recordBadge, _badgeFadeDuration, _badgeEase).Forget();
            PlayPS(_recordBadgeVFX).Forget();
        }
    }

    public void Reset()
    {
        var color = _text.color;
        color.a = 0;
        _text.color = color;
        
        _text.text = _startCount.ToString(CultureInfo.InvariantCulture);
        
        SetAlpha(_icon);
        SetAlpha(_recordBadge);
    }
    
    public async UniTask TweenValue(GameObject element, float start, float end, float duration, LeanTweenType ease, Action<float> onUpdate)
    {
        var completion = new UniTaskCompletionSource();

        LeanTween.value(gameObject, start, end, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate(onUpdate)
            .setOnComplete(() => completion.TrySetResult());

        await completion.Task;
    }

    private void SetAlpha(Image image, float alpha = 0)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private async UniTask ShowIcon(Image icon, float duration, LeanTweenType ease, float from = 0, float to = 1)
    {
        await TweenValue(
            icon.gameObject,
            from,
            to,
            duration,
            ease,
            (value) =>
            {
                SetAlpha(icon, value);
            }
        );
    }
    
    private async UniTask PlayPS(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        particle.Play();
        
        await UniTask.WaitUntil(() => !particle.IsAlive());
        
        particle.Stop();
        particle.gameObject.SetActive(false);
    }
}
