using UnityEngine;
using UnityEngine.UI;

public class ShowHideSettings : MonoBehaviour
{

    public static ShowHideSettings Instance { get; private set; }
    
    // ----------------------------------------
    // REFERENCES
    // ----------------------------------------
    [Header("References")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup settingsGroup;
    public CanvasGroup creditsGroup;
    public CanvasGroup exitGroup;
    public float fadeTime = 0.3f;

    [Header("Volume")]
    public Slider volumeSlider;

    // ----------------------------------------
    // UNITY EVENTS
    // ----------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initialize settings, credits, bugs panels as hidden and non-interactable
        if (settingsGroup != null)
        {
            settingsGroup.alpha = 0;
            settingsGroup.interactable = false;
            settingsGroup.blocksRaycasts = false;
        }

        if (creditsGroup != null)
        {
            creditsGroup.alpha = 0;
            creditsGroup.interactable = false;
            creditsGroup.blocksRaycasts = false;
        }

        if (exitGroup != null)
        {
            exitGroup.alpha = 0;
            exitGroup.interactable = false;
            exitGroup.blocksRaycasts = false;
        }
    }

    // ----------------------------------------
    // PANEL CONTROLS
    // ----------------------------------------
    public void ShowSettings()
    {
        if (settingsGroup == null || mainMenuGroup == null) return;

        // Show settings panel, disable main menu interaction
        settingsGroup.alpha = 1;
        settingsGroup.interactable = true;
        settingsGroup.blocksRaycasts = true;

        mainMenuGroup.interactable = false;
    }

    public void HideSettings()
    {
        if (settingsGroup == null || mainMenuGroup == null) return;

        // Hide settings panel, re-enable main menu interaction
        settingsGroup.alpha = 0;
        settingsGroup.interactable = false;
        settingsGroup.blocksRaycasts = false;

        mainMenuGroup.interactable = true;
    }

    public void ShowCredits()
    {
        if (creditsGroup == null || mainMenuGroup == null) return;

        // Show credits panel, disable main menu interaction
        creditsGroup.alpha = 1;
        creditsGroup.interactable = true;
        creditsGroup.blocksRaycasts = true;

        mainMenuGroup.interactable = false;
    }

    public void HideCredits()
    {
        if (creditsGroup == null || mainMenuGroup == null) return;

        // Hide credits panel, re-enable main menu interaction
        creditsGroup.alpha = 0;
        creditsGroup.interactable = false;
        creditsGroup.blocksRaycasts = false;

        mainMenuGroup.interactable = true;
    }

    public void ShowExit()
    {
        if (exitGroup == null || mainMenuGroup == null) return;

        // Show exit panel, disable main menu interaction
        exitGroup.alpha = 1;
        exitGroup.interactable = true;
        exitGroup.blocksRaycasts = true;

        mainMenuGroup.interactable = false;
    }

    public void HideExit()
    {
        if (exitGroup == null || mainMenuGroup == null) return;

        // Hide exit panel, re-enable main menu interaction
        exitGroup.alpha = 0;
        exitGroup.interactable = false;
        exitGroup.blocksRaycasts = false;

        mainMenuGroup.interactable = true;
    }

    // ----------------------------------------
    // VOLUME SETTINGS
    // ----------------------------------------
    public void SetVolume()
    {
        if (volumeSlider == null) return;

        // Update global volume and save setting
        AudioListener.volume = volumeSlider.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        if (volumeSlider == null) return;

        PlayerPrefs.SetFloat("soundVolume", volumeSlider.value);
    }

    public void LoadVolume()
    {
        if (volumeSlider == null) return;

        // Load saved volume or default to 1
        volumeSlider.value = PlayerPrefs.GetFloat("soundVolume", 1f);
    }

    // ----------------------------------------
    // VIDEO SETTINGS
    // ----------------------------------------

    //public void ToggleFS()
    //{
    //    if(Screen.fullScreen)
    //    {
    //        Screen.SetResolution(1024, 768, false);
    //        SoundManager.Instance.PlayToggleOFFSFX();
    //    }
    //    else
    //    {
    //        Resolution currentRes = Screen.currentResolution;
    //        Screen.SetResolution(currentRes.width, currentRes.height, true);
    //        SoundManager.Instance.PlayToggleONSFX();
    //    }
    //}
}
