using UnityEngine;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [SerializeField] private LocalizationDatabase_SO database;
    [SerializeField] private Language currentLanguage = Language.English;

    public static event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static string Get(string key)
    {
        return Instance.database.Get(key, Instance.currentLanguage);
    }

    public void SetLanguage(Language language)
    {
        if (currentLanguage == language)
            return;

        currentLanguage = language;
        OnLanguageChanged?.Invoke();
    }

    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }
}
