using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<TabElement> _tabButtons;
    private TabElement _currentTab;
    private RectTransform _rectTransform;

    void Awake() => _rectTransform = GetComponent<RectTransform>();

    void Start()
    {
        foreach (var tab in _tabButtons)
        {
            if (tab == null) continue;
            if (tab.TabType == TabType.Main) _currentTab = tab;
            tab.DeselectTab();
        }
        
        if (_currentTab != null) _currentTab.SelectTab();
        
        LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
    }

    public void SubscribeToGroup(TabElement tabButton)
    {
        if (_tabButtons == null) _tabButtons = new List<TabElement>();
        
        if (tabButton != null && !_tabButtons.Contains(tabButton))
            _tabButtons.Add(tabButton);
    }

    public void OnTabSelected(TabElement clicked)
    {
        if (clicked == null || _currentTab == clicked) return;
        _currentTab = clicked;

        foreach (var tab in _tabButtons)
        {
            if (tab == null) continue;
            if (tab == clicked) tab.SelectTab();
            else tab.DeselectTab();
        }

        LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
    }
}