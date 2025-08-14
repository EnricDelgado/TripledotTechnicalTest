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

    [Header("Tab Values")]
    [SerializeField] private float _verticalOffset = 10f;
    [SerializeField] private float _scaleFactor = 1.2f;
    [SerializeField] private TabType _tabType;
    
    [Header("Animation Clips")]
    [SerializeField] private IconAnimClip _selectClip;
    [SerializeField] private IconAnimClip _deselectClip;

    private AnimatedLayoutElement _layout;
    private TabState _currentState = TabState.Unselected;

    public TabType TabType => _tabType;


    
    void Awake()
    {
        _layout = GetComponent<AnimatedLayoutElement>();
        _layout.Element.preferredWidth = _collapsedWidth;
    }

    void Start() => DeselectTab();
    void OnEnable() => _tabGroup.SubscribeToGroup(this);

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
        if (_currentState == TabState.Selected) return;
        
        _background.TweenIconAlpha(1, CancellationToken.None).Forget();

        /*
         _icon.TweenIconPosition(
            new Vector3(
                _icon.RectTransform.localPosition.x, 
                _icon.RectTransform.localPosition.y + _verticalOffset,
                _icon.RectTransform.localPosition.z
            ), 
            CancellationToken.None
        ).Forget();
        _icon.TweenIconScale(Vector3.one * _scaleFactor, CancellationToken.None).Forget();        
        */

        _icon.PlayClip(_selectClip, CancellationToken.None).Forget();

        _text.TweenTextVisibility(1, CancellationToken.None);
        
        _layout.TweenWidthAsync(_expandedWidth, CancellationToken.None);
        _layout.Element.layoutPriority = -1;
        
        _currentState = TabState.Selected;
    }

    public void DeselectTab()
    {
        if (_tabType == TabType.Locked) return;
        if (_currentState == TabState.Unselected) return;
        
        /*_icon.ResetIconPosition(CancellationToken.None).Forget();
        _icon.ResetIconScale(CancellationToken.None).Forget();*/
        _icon.PlayClip(_deselectClip, CancellationToken.None).Forget();
        
        _background.TweenIconAlpha(0, CancellationToken.None).Forget();
        _text.TweenTextVisibility(0, CancellationToken.None);
        _layout.TweenWidthAsync(_collapsedWidth, CancellationToken.None);

        _layout.Element.layoutPriority = 1;
        
        _currentState = TabState.Unselected;
    }
    
    public void SetButtonLocked() => Debug.Log("[EDC] Button locked");
}
