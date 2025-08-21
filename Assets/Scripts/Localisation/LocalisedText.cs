using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalisedText : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private LocalisationManager localisationManager;

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
        textComponent.text = localisationManager.GetText(key);
    }
}