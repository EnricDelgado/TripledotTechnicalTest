using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private LocalisationManager _localisationManager;

    private List<string> _langCodes;

    private void Start()
    {
        PopulateOptions();
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        LocalisationManager.OnLanguageChanged += UpdateDropdownValue;
    }

    private void OnDestroy()
    {
        LocalisationManager.OnLanguageChanged -= UpdateDropdownValue;
    }

    private void PopulateOptions()
    {
        dropdown.ClearOptions();

        _langCodes = new List<string>();
        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var pair in _localisationManager.LanguageNames)
        {
            _langCodes.Add(pair.Key);
            options.Add(new TMP_Dropdown.OptionData(pair.Value));
        }

        dropdown.AddOptions(options);
        UpdateDropdownValue();
    }

    private void UpdateDropdownValue()
    {
        var currentLangIndex = _langCodes.IndexOf(_localisationManager.CurrentLanguageCode);
        if (currentLangIndex >= 0)
            dropdown.value = currentLangIndex;
    }

    private void OnDropdownChanged(int index)
    {
        if (index >= 0 && index < _langCodes.Count)
        {
            _localisationManager.SetLanguage(_langCodes[index]);
        }
    }
}