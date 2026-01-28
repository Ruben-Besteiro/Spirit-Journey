using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;

    [Header("Audio Preview")]
    [SerializeField] private AudioUnit_SO sfxPreview;
    [SerializeField] private AudioUnit_SO voicePreview;

    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Coroutine sfxPreviewRoutine;
    private Coroutine voicePreviewRoutine;

    private void Start()
    {
        InitializeUI();
        RegisterListeners();
    }

    private void InitializeUI()
    {
        /* AUDIO */

        musicSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetMusicVolume()
        );

        sfxSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetSFXVolume()
        );

        voiceSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetVoiceVolume()
        );

        /* LANGUAGE */

        SetupLanguageDropdown();

        Language currentLang = LocalizationManager.Instance.GetCurrentLanguage();
        languageDropdown.SetValueWithoutNotify(
            currentLang == Language.Spanish ? 1 : 0
        );

        /* RESOLUTION */

        SetupResolutionDropdown();

        resolutionDropdown.SetValueWithoutNotify(
            GetCurrentResolutionIndex()
        );
    }

    private int GetCurrentResolutionIndex()
    {
        if (Screen.fullScreenMode != FullScreenMode.Windowed)
            return 0;

        if (Screen.width == 1920 && Screen.height == 1080) return 1;
        if (Screen.width == 1280 && Screen.height == 720) return 2;
        if (Screen.width == 854 && Screen.height == 480) return 3;
        if (Screen.width == 568 && Screen.height == 320) return 4;

        return 1;
    }

    private void RegisterListeners()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        voiceSlider.onValueChanged.AddListener(OnVoiceChanged);

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    /* AUDIO */

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnVoiceChanged(float value)
    {
        AudioManager.Instance.SetVoiceVolume(value);
    }

    /* LANGUAGE */

    private void SetupLanguageDropdown()
    {
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string>()
        {
            "English",
            "Español"
        });

        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int index)
    {
        Language language = index switch
        {
            1 => Language.Spanish,
            _ => Language.English
        };

        LocalizationManager.Instance.SetLanguage(language);
    }

    /* RESOLUTION */

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(new List<string>()
        {
            "Fullscreen",
            "1920 x 1080",
            "1280 x 720",
            "854 x 480",
            "568 x 320"
        });

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void OnResolutionChanged(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(Screen.currentResolution.width,
                                     Screen.currentResolution.height,
                                     FullScreenMode.FullScreenWindow);
                break;

            case 1:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;

            case 2:
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;

            case 3:
                Screen.SetResolution(854, 480, FullScreenMode.Windowed);
                break;

            case 4:
                Screen.SetResolution(568, 320, FullScreenMode.Windowed);
                break;
        }
    }
}
