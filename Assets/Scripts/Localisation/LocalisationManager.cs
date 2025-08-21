using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationManager : MonoBehaviour
{
    public static LocalisationManager Instance { get; private set; }

    private Dictionary<string, string> currentLanguageTable;
    private Dictionary<string, Dictionary<string, string>> allLanguages;

    [SerializeField] private string defaultLanguage = "en";
    
    public static event Action OnLanguageChanged;
    

    [System.Serializable]
    private class LocalisationWrapper
    {
        public Language[] languages;
    }

    [System.Serializable]
    private class Language
    {
        public string code;
        public Entry[] entries;
    }

    [System.Serializable]
    private class Entry
    {
        public string key;
        public string value;
    }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguages();
            SetLanguage(defaultLanguage);
        } else {
            Destroy(gameObject);
        }
    }

    private void LoadLanguages()
    {
        var json = Resources.Load<TextAsset>("localisation").text;
        var wrapper = JsonUtility.FromJson<LocalisationWrapper>(json);

        allLanguages = new Dictionary<string, Dictionary<string, string>>();

        foreach (var lang in wrapper.languages)
        {
            var table = new Dictionary<string, string>();
            foreach (var entry in lang.entries)
                table[entry.key] = entry.value;

            allLanguages[lang.code] = table;
        }
    }

    public void SetLanguage(string langCode)
    {
        if (allLanguages.ContainsKey(langCode))
            currentLanguageTable = allLanguages[langCode];
        else {
            Debug.LogWarning($"Language {langCode} not found, fallback to {defaultLanguage}");
            currentLanguageTable = allLanguages[defaultLanguage];
        }

        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) 
            return "#MISSING_KEY#";

        key = key.Trim();

        return currentLanguageTable != null && currentLanguageTable.ContainsKey(key)
            ? currentLanguageTable[key]
            : $"#{key}#";
    }
}