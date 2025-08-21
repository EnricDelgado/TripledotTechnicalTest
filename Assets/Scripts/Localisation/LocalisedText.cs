using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalisedText : MonoBehaviour
{
    [SerializeField] private string _key;
    [SerializeField] private TextMeshProUGUI _textComponent;
    [SerializeField] private LocalisationManager _localisationManager;

    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        LocalisationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        LocalisationManager.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        _textComponent.text = _localisationManager.GetText(_key);
    }
}