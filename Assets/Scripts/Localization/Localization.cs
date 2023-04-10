using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Localization
{
    #region Singleton

    private static Localization _instance;

    private Localization()
    {
    }

    public static Localization Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Localization();
                _instance.Initialize();
            }

            return _instance;
        }
    }

    #endregion

    private const string ERROR = "*** INVALID KEY ***";
    
    private static readonly Dictionary<string, string> LocalizationDictionary = new();
    private static string _currentLanguage = "en";
    private static Action _onLanguageChange;

    private void Initialize()
    {
        LoadLocalization();
    }

    private void LoadLocalization()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "localization.tsv");
        var file = File.ReadAllText(path);
        var records = file.Split('\n');
        foreach (var record in records)
        {
            var columns = record.Split('\t');
            LocalizationDictionary.Add(columns[0] + "-en", columns[3]);
            LocalizationDictionary.Add(columns[0] + "-fr", columns[4]);
            LocalizationDictionary.Add(columns[0] + "-es", columns[5]);
            LocalizationDictionary.Add(columns[0] + "-ru", columns[6]);
            LocalizationDictionary.Add(columns[0] + "-sc", columns[7]);
        }

        _onLanguageChange?.Invoke();
    }

    public void ChangeLanguage(string _language)
    {
        _currentLanguage = _language;
        _onLanguageChange?.Invoke();
    }

    public string GetLocalization(string _key)
    {
        return LocalizationDictionary.TryGetValue(_key + "-" + _currentLanguage, out var text) ? text : ERROR;
    }

    public void Subscribe(Action _action)
    {
        _onLanguageChange += _action;
    }

    public void Unsubscribe(Action _action)
    {
        _onLanguageChange -= _action;
    }
}

[Serializable]
public class LanguageData
{
    public string key;
    public string en;
    public string fr;
    public string es;
    public string ru;
    public string sc;
}

[Serializable]
public class StringsData
{
    public LanguageData[] strings;
}