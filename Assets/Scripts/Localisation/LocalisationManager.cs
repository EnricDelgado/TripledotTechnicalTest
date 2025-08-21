using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationManager : MonoBehaviour
{
    public static event Action OnLanguageChanged;
    public Dictionary<string, string> LanguageNames = new ();
    public string CurrentLanguageCode { get; private set; }
    
    public static LocalisationManager Instance { get; private set; }

    private Dictionary<string, string> _currentLanguageTable;
    private Dictionary<string, Dictionary<string, string>> _allLanguages;

    [SerializeField] private string _defaultLanguage = "en";
    
    

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
            SetLanguage(_defaultLanguage);
        } else {
            Destroy(gameObject);
        }
    }

    private void LoadLanguages()
    {
        var json = Resources.Load<TextAsset>("localisation").text;
        var wrapper = JsonUtility.FromJson<LocalisationWrapper>(json);

        _allLanguages = new Dictionary<string, Dictionary<string, string>>();

        foreach (var lang in wrapper.languages)
        {
            var table = new Dictionary<string, string>();
            foreach (var entry in lang.entries)
                table[entry.key] = entry.value;

            _allLanguages[lang.code] = table;
            
            if (table.ContainsKey($"lang_{lang.code}"))
                LanguageNames[lang.code] = table[$"lang_{lang.code}"];
            else
                LanguageNames[lang.code] = lang.code; // fallback
        }
    }

    public void SetLanguage(string langCode)
    {
        if (_allLanguages.ContainsKey(langCode))
        {
            _currentLanguageTable = _allLanguages[langCode];
            CurrentLanguageCode = langCode;
        }
        else {
            Debug.LogWarning($"Language {langCode} not found, fallback to {_defaultLanguage}");
            _currentLanguageTable = _allLanguages[_defaultLanguage];
            CurrentLanguageCode = _defaultLanguage;
        }
        
        print($"[LOCALISATION] Setting lang to: {langCode}");
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) 
            return "#MISSING_KEY#";

        key = key.Trim();

        return _currentLanguageTable != null && _currentLanguageTable.ContainsKey(key)
            ? _currentLanguageTable[key]
            : $"#{key}#";
    }
    
    public List<string> GetAvailableLanguages()
    {
        return new List<string>(_allLanguages.Keys);
    }
}