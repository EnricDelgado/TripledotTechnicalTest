using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EndGameModal : BaseModal
{
    [Header("Texts")]
    [Header("Text Objects")]
    [SerializeField] private RectTransform _textHolder;
    [SerializeField] private RectTransform _textA;
    [SerializeField] private RectTransform _textB;

    [Header("Text Animation Attributes")]
    [SerializeField] private float _textEnterDelay = .3f;
    [SerializeField] private float _textTranslateDelay = .47f;
    [SerializeField] private float _textEnterDuration = .23f;
    [SerializeField] private float _textTranslateDuration = .4f;
    [SerializeField] private float _textAFinalPos = 0;
    [SerializeField] private float _textBFinalPos = 0;
    [SerializeField] private float _textHolderFinalPos = -550;
    [SerializeField] private float _effectEnterDelay = .3f;
    [SerializeField] private LeanTweenType _textEnterEase = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType _textTranslateEase = LeanTweenType.easeOutBack;
    
    [Header("Background")]
    [SerializeField] private RectTransform _background;
    [SerializeField] private float _backgroundEnterDelay = .3f;
    [SerializeField] private float _backgroundEnterDuration = .47f;
    [SerializeField] private LeanTweenType _backgroundEase = LeanTweenType.easeOutBack;
    
    [Header("Stars")]
    [Header("Star Objects")]
    [SerializeField] private CanvasGroup _starGroup;
    [SerializeField] private GrowingIcon[] _stars;
    [Range(0,3)][SerializeField] private int _obtainedStars;
    
    [Header("Star Animation Attributes")]
    [SerializeField] private float _starFadeInDuration = .3f;
    [SerializeField] private float _nextStarDelay = .21f;
    [SerializeField] private LeanTweenType _starsEase = LeanTweenType.easeOutBack;
    
    [Header("Notch")]
    [SerializeField] private RectTransform _notch;
    [SerializeField] private float _notchRevealDuration = 0.18f;
    [SerializeField] private float _notchRevealDelay = .2f;
    [SerializeField] private LeanTweenType _notchEase = LeanTweenType.easeOutBack;
    
    [Header("Endgame Counters")]
    [SerializeField] private EndgameCounter[] _counters;
    [SerializeField] private float _counterEnterDelay = .47f;
    
    [Header("Buttons")]
    [SerializeField] private RectTransform _button;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem _backgroundVFX;
    [SerializeField] private ParticleSystem _buttonVFX;

    private float _initialTextScale;
    private Vector2 _initialTextAPos;
    private Vector2 _initialTextBPos;
    private Vector2 _initialTextHolderPos;

    private Vector2 _initialBackgroundOffset;
    private Vector2 _initialNotchOffset;

    private void Start()
    {
        _initialTextScale = _textA.localScale.x;
        _initialTextAPos = _textA.anchoredPosition;
        _initialTextBPos = _textB.anchoredPosition;
        _initialTextHolderPos = _textHolder.anchoredPosition;
        _initialBackgroundOffset = _background.offsetMin;
        _initialNotchOffset = _notch.offsetMin;
    }
    
    public override void ShowModal()
    {
        base.ShowModal();
        SetupScreen().Forget();
    }
    
    public override void HideModal()
    {
        base.HideModal();
        
        _textHolder.anchoredPosition = _initialTextHolderPos;
        _textA.anchoredPosition = _initialTextAPos;
        _textB.anchoredPosition = _initialTextBPos;
        
        _background.offsetMin = _initialBackgroundOffset;
        
        _starGroup.alpha = 0;
        foreach (var star in _stars)
            star.ResetIcon();
        
        _notch.offsetMin = _initialNotchOffset;
        foreach(var counter in _counters)
            counter.Reset();
        
        _button.gameObject.SetActive(false);

        _backgroundVFX.Stop();
        _backgroundVFX.gameObject.SetActive(false);
    }

    private async UniTask SetupScreen()
    {
        await EnterText();
        
        await UnrollElement(_background, 0, _backgroundEnterDuration, _backgroundEnterDelay, _backgroundEase);

        await FadeGroupIn(_starGroup, _starFadeInDuration, _starsEase);

        if (_obtainedStars > 0)
        {
            for(int i = 0; i < _obtainedStars; i++)
                await UnlockStar(i);
        }

        // Notch
        await UnrollElement(_notch, 260, _notchRevealDuration, _notchRevealDelay, _notchEase);

        // Loot
        for (int i = 0; i < _counters.Length; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_counterEnterDelay));
            await _counters[i].Enter();
        }

        // Button
        // Show button with VFX
        _button.gameObject.SetActive(true);
        PlayPS(_buttonVFX).Forget();
        

        // --- Button ---
        // TODO: add your button reveal VFX
    }

    private async UniTask EnterText()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_textEnterDelay));
        
        // Animated Text
        await TranslateElementAxis(_textA, Vector2.left,  _textAFinalPos, _textEnterDuration, _textEnterEase);
        await TranslateElementAxis(_textB, Vector2.right, _textBFinalPos, _textEnterDuration, _textEnterEase);
        
        await UniTask.Delay(TimeSpan.FromSeconds(_textTranslateDelay));
        
        await UniTask.WhenAll(
            TranslateElementAxis(_textHolder, Vector2.up, _textHolderFinalPos, _textTranslateDuration),
            TweenValue(
                _initialTextScale,
                1,
                _textTranslateDuration,
                _textTranslateEase,
                (float value) =>
                {
                    var newScale = Vector3.one * value;
                    _textA.transform.localScale = newScale;
                    _textB.transform.localScale = newScale;
                }
            )
        );
            
        await UniTask.Delay(TimeSpan.FromSeconds(_effectEnterDelay));
        _backgroundVFX.gameObject.SetActive(true);
        _backgroundVFX.Play();
    }

    private async UniTask UnrollElement(RectTransform element, float end, float duration, float delay, LeanTweenType ease)
    {
        var completionSource = new UniTaskCompletionSource();

        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        float startHeight = element.offsetMin.y;

        LeanTween.value(element.gameObject, startHeight, end, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) =>
            {
                var offset = element.offsetMin;
                offset.y = value;
                element.offsetMin = offset;
            })
            .setOnComplete(() =>
            {
                completionSource.TrySetResult();
            });

        await completionSource.Task;
    }
    
    private async UniTask TranslateElementAxis(RectTransform element, Vector2 axis, float targetPos, float duration, LeanTweenType easeType = LeanTweenType.easeOutBack)
    {
        var completionSource = new UniTaskCompletionSource();
        
        // axis should be an enum
        if (axis == Vector2.down || axis == Vector2.up)
        {
            LeanTween.moveY(element, targetPos, duration)
                .setEase(easeType)
                .setIgnoreTimeScale(true)
                .setOnComplete((() => completionSource.TrySetResult()));
        }
        
        if(axis == Vector2.left || axis == Vector2.right)
        {
            LeanTween.moveX(element, targetPos, duration)
                .setEase(easeType)
                .setIgnoreTimeScale(true)
                .setOnComplete((() => completionSource.TrySetResult()));
        }

        
        await completionSource.Task;
    }

    public async UniTask TweenValue(float start, float end, float duration, LeanTweenType ease, Action<float> onUpdate)
    {
        var completion = new UniTaskCompletionSource();

        LeanTween.value(start, end, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate(onUpdate)
            .setOnComplete(() => completion.TrySetResult());

        await completion.Task;
    }
    
    private async UniTask FadeGroupIn(CanvasGroup canvasGroup, float duration, LeanTweenType easeType = LeanTweenType.easeOutBack)
    {
        var completionSource = new UniTaskCompletionSource();

        LeanTween.value(canvasGroup.gameObject, 0, 1, duration)
            .setEase(_textEnterEase)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) =>
            {
                canvasGroup.alpha = value;
            })
            .setOnComplete(() =>
            {
                completionSource.TrySetResult();
            });
        
        await completionSource.Task;
    }

    private async UniTask UnlockStar(int starIndex)
    {
        await _stars[starIndex].GrowIcon();
        await UniTask.Delay(TimeSpan.FromSeconds(_nextStarDelay));
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
