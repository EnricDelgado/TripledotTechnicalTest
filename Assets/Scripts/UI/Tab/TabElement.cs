using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class TabEvent : UnityEvent { }

[RequireComponent(typeof(LayoutElement))]
public class TabElement : MonoBehaviour, IPointerClickHandler
{
    [Header("Tab Elements")]
    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private TabType _tabType;
    [SerializeField] private TweenedElement _background;
    [SerializeField] private TweenedElement _icon;
    [SerializeField] private TweenedElement _text;
    
    [Header("Animation Clips")]
    [SerializeField] private TweenClip _iconSelectClip;
    [SerializeField] private TweenClip _backgroundSelectClip;
    [SerializeField] private TweenClip _textSelectClip;
    [SerializeField] private TweenClip _layoutSelectClip;
    [SerializeField] private TweenClip _iconDeselectClip;
    [SerializeField] private TweenClip _backgroundDeselectClip;
    [SerializeField] private TweenClip _textDeselectClip;
    [SerializeField] private TweenClip _layoutDeselectClip;
    [SerializeField] private TweenClip _lockedTabClip;

    [Header("Action Trigger")] 
    [SerializeField] private TabEvent _tabEnterEvent;
    [SerializeField] private TabEvent _tabExitEvent;
    [SerializeField] private TabEvent _tabLockedEvent;

    private AnimatedLayoutElement _layout;
    private TabState _tabState = TabState.Unselected;
    
    public TabType TabType => _tabType;

    
    void Awake() => _layout = GetComponent<AnimatedLayoutElement>();
    void OnEnable() => _tabGroup.SubscribeToGroup(this);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_tabType == TabType.Locked)
            LockedTab();
        else 
            _tabGroup.OnTabSelected(this);
    }

    public void SelectTab()
    {
        if (_tabState == TabState.Selected) return;
        
        _icon.PlayClip(_iconSelectClip, CancellationToken.None).Forget();
        _background.PlayClip(_backgroundSelectClip, CancellationToken.None).Forget();
        _text.PlayClip(_textSelectClip, CancellationToken.None).Forget();
        _layout.PlayClip(_layoutSelectClip, CancellationToken.None).Forget();
        
        _layout.Element.layoutPriority = -1;
        
        _tabState = TabState.Selected;
        
        _tabEnterEvent?.Invoke();
    }

    public void DeselectTab()
    {
        if (_tabState == TabState.Unselected) return;
        
        _icon.PlayClip(_iconDeselectClip, CancellationToken.None).Forget();
        _background.PlayClip(_backgroundDeselectClip, CancellationToken.None).Forget();
        _text.PlayClip(_textDeselectClip, CancellationToken.None).Forget();
        _layout.PlayClip(_layoutDeselectClip, CancellationToken.None).Forget();

        _layout.Element.layoutPriority = 1;
        
        _tabState = TabState.Unselected;
        
        _tabExitEvent?.Invoke();
    }
    
    private void LockedTab()
    {
        _icon.PlayClip(_lockedTabClip, CancellationToken.None).Forget();
        _tabLockedEvent?.Invoke();
    }
}
