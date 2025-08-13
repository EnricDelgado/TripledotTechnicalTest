using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LayoutElement))]
public class TabElement : MonoBehaviour, IPointerClickHandler
{
    [Header("Tab Elements")]
    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private AnimatedIcon _background;
    [SerializeField] private AnimatedIcon _icon;
    [SerializeField] private AnimatedText _text;

    [Header("Tab Sizing")]
    [SerializeField] private float _expandedWidth = 220f;
    [SerializeField] private float _collapsedWidth = 120f;
    [SerializeField] private float _resizeDuration = 0.18f;
    [SerializeField] private AnimationCurve _resizeCurve = AnimationCurve.EaseInOut(0,0,1,1);

    [Header("Tab Values")]
    [SerializeField] private float _verticalOffset = 10f;
    [SerializeField] private float _scaleFactor = 1.2f;
    [SerializeField] private TabType _tabType;

    public TabType TabType => _tabType;

    AnimatedLayoutElement _layout;
    Vector3 _iconScale0;
    Vector3 _iconLocalPos0;

    // We keep one CTS per element to cancel any running animation when a new one starts
    CancellationTokenSource _animCts;

    // Track tween ids so we can cancel precisely
    int _widthTweenId = -1;
    int _scaleTweenId = -1;
    int _moveTweenId  = -1;

    
    void Awake()
    {
        _layout = GetComponent<AnimatedLayoutElement>();
        _layout.Element.preferredWidth = _collapsedWidth;
    }

    void Start() => ApplyVisualCollapsedImmediate();
    void OnEnable() => _tabGroup.SubscribeToGroup(this);
    void OnDisable() => CancelAnimation();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_tabType == TabType.Locked)
        {
            SetButtonLocked();
            return;
        }
        
        _tabGroup.OnTabSelected(this);
    }

    public void SelectTab()
    {
        if (_background) _background.gameObject.SetActive(true);
        if (_text) _text.gameObject.SetActive(true);

        // fire-and-forget with UniTask, but cancellable
        //AnimateToAsync(_expandedWidth, _scaleFactor, _verticalOffset).Forget();
        
        // Animate Background Alpha to 1
        _background.TweenIconAlphaAsync(1, .2f, CancellationToken.None).Forget();
        
        // Animate Icon scale and position synchronously
        _icon.TweenIconMoveAsync(
            new Vector3(
                _icon.RectTransform.localPosition.x, 
                _icon.RectTransform.localPosition.y + _verticalOffset,
                _icon.RectTransform.localPosition.z
            ), 
            CancellationToken.None
        ).Forget();
        
        _icon.TweenIconScaleAsync(Vector3.one * _scaleFactor, CancellationToken.None).Forget();
        
        // Animate text alpha to 1
        _text.TweenTextVisibility(1, CancellationToken.None);
        
        // Animate LayoutElement width
        _layout.TweenWidthAsync(_expandedWidth, CancellationToken.None);

        _layout.Element.layoutPriority = 1;
    }

    public void DeselectTab()
    {
        if (_tabType == TabType.Locked) return;
        //AnimateToAsync(_collapsedWidth, 1f, 0f, ApplyVisualCollapsedImmediate).Forget();
        
        // revert everything and go back to normal
        _background.TweenIconAlphaAsync(0, .2f, CancellationToken.None).Forget();
        _icon.ResetIconPositionAsync(CancellationToken.None).Forget();
        _icon.ResetIconScaleAsync(CancellationToken.None).Forget();
        _text.TweenTextVisibility(0, CancellationToken.None);
        _layout.TweenWidthAsync(_collapsedWidth, CancellationToken.None);

        _layout.Element.layoutPriority = 1;
    }

    void ApplyVisualCollapsedImmediate()
    {
        if (_background) _background.gameObject.SetActive(false);
        if (_text) _text.gameObject.SetActive(false);
    }

    void CancelAnimation()
    {
        _animCts?.Cancel();
        _animCts?.Dispose();
        _animCts = null;

        if (_widthTweenId >= 0) { LeanTween.cancel(_widthTweenId); _widthTweenId = -1; }
        if (_scaleTweenId >= 0) { LeanTween.cancel(_scaleTweenId); _scaleTweenId = -1; }
        if (_moveTweenId  >= 0) { LeanTween.cancel(_moveTweenId);  _moveTweenId  = -1; }
    }
    
    public void SetButtonLocked() => Debug.Log("[EDC] Button locked");
}
