using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationManager
{
    private static LocalizationManager _instance;

    private static readonly object _lock = new object();
    public static LocalizationManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }
    }

    private LocalizationManager()
    {
        
    }

    public event Action OnLanguageChanged;

    public string Language => LocalizationSettings.SelectedLocale.Identifier.Code;

    public void SetLanguage(string localeCode)
    {
        var locale = GetLocale(localeCode);
        SetLanguage(locale);
    }

    public string GetLocalizedString(string tableName, string entryKey)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, entryKey);
    }

    public async UniTask<string> GetLocalizedStringAsync(string tableName, string entryKey)
    {
        return await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, entryKey);
    }

    private void SetLanguage(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
        OnLanguageChanged?.Invoke();
    }

    private Locale GetLocale(string localeCode)
    {
        Locale locale = LocalizationSettings.AvailableLocales
                                            .Locales
                                            .FirstOrDefault(locale => locale.Identifier.Code == localeCode);
        if (locale == null)
        {
            Debug.LogWarning($"No locale with code '{localeCode}' was found");
        }

        return locale;
    }
}
