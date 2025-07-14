using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShowHideSettings : MonoBehaviour
{

    public static ShowHideSettings Instance { get; private set; }

    ControllerMouseSwitch deviceSwitch;
    
    // ----------------------------------------
    // REFERENCES
    // ----------------------------------------
    [Header("References")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup settingsGroup;
    public CanvasGroup creditsGroup;
    public CanvasGroup exitGroup;
    public float fadeTime = 0.3f;

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

        deviceSwitch = FindFirstObjectByType<ControllerMouseSwitch>();
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

        deviceSwitch.SetSelectedOnSettings();
    }

    public void HideSettings()
    {
        if (settingsGroup == null || mainMenuGroup == null) return;

        // Hide settings panel, re-enable main menu interaction
        settingsGroup.alpha = 0;
        settingsGroup.interactable = false;
        settingsGroup.blocksRaycasts = false;

        mainMenuGroup.interactable = true;

        deviceSwitch.SetSelectedOnMenu();
    }

    public void ShowCredits()
    {
        if (creditsGroup == null || mainMenuGroup == null) return;

        // Show credits panel, disable main menu interaction
        creditsGroup.alpha = 1;
        creditsGroup.interactable = true;
        creditsGroup.blocksRaycasts = true;

        mainMenuGroup.interactable = false;
        settingsGroup.interactable = false;
    }

    public void HideCredits()
    {
        if (creditsGroup == null || mainMenuGroup == null) return;

        // Hide credits panel, re-enable main menu interaction
        creditsGroup.alpha = 0;
        creditsGroup.interactable = false;
        creditsGroup.blocksRaycasts = false;

        settingsGroup.interactable = true;
    }

    public void ShowExit()
    {
        if (exitGroup == null || mainMenuGroup == null) return;

        // Show exit panel, disable main menu interaction
        exitGroup.alpha = 1;
        exitGroup.interactable = true;
        exitGroup.blocksRaycasts = true;

        mainMenuGroup.interactable = false;

        deviceSwitch.SetSelectedOnExit();
    }

    public void HideExit()
    {
        if (exitGroup == null || mainMenuGroup == null) return;

        // Hide exit panel, re-enable main menu interaction
        exitGroup.alpha = 0;
        exitGroup.interactable = false;
        exitGroup.blocksRaycasts = false;

        mainMenuGroup.interactable = true;

        deviceSwitch.SetSelectedOnMenu();
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
