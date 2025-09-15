using UnityEngine;

public class ShowHideSettings : MonoBehaviour
{
    [Header("CanvasGroups")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup settingsGroup;
    public CanvasGroup creditsGroup;
    public CanvasGroup exitGroup;
    public CanvasGroup saveDeleteGroup;

    [Header("UI Device")]
    public ControllerMouseSwitch deviceSwitch;

    private void Awake()
    {
        // Optionally, assign deviceSwitch if not set in inspector
        if (deviceSwitch == null)
            deviceSwitch = FindFirstObjectByType<ControllerMouseSwitch>();
    }

    private void Start()
    {
        AssignCanvasGroups();
        HideAllPanelsImmediate();
    }

    /// <summary>
    /// Assign CanvasGroups dynamically if not set in inspector
    /// </summary>
    public void AssignCanvasGroups()
    {
        if (mainMenuGroup == null)
            mainMenuGroup = GameObject.Find("MainMenuGroup")?.GetComponent<CanvasGroup>();
        if (settingsGroup == null)
            settingsGroup = GameObject.Find("SettingsGroup")?.GetComponent<CanvasGroup>();
        if (creditsGroup == null)
            creditsGroup = GameObject.Find("CreditsGroup")?.GetComponent<CanvasGroup>();
        if (exitGroup == null)
            exitGroup = GameObject.Find("ExitGroup")?.GetComponent<CanvasGroup>();
        if (saveDeleteGroup == null)
            saveDeleteGroup = GameObject.Find("SaveDeleteGroup")?.GetComponent<CanvasGroup>();
    }

    private void HideAllPanelsImmediate()
    {
        HideCanvas(settingsGroup);
        HideCanvas(creditsGroup);
        HideCanvas(exitGroup);
        HideCanvas(saveDeleteGroup);
    }

    private void HideCanvas(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private void ShowCanvas(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    // -----------------------------
    // Panel Controls
    // -----------------------------
    public void ShowSettings()
    {
        ShowCanvas(settingsGroup);
        mainMenuGroup.interactable = false;
        deviceSwitch?.SetSelectedOnSettings();
    }

    public void HideSettings()
    {
        HideCanvas(settingsGroup);
        mainMenuGroup.interactable = true;
        deviceSwitch?.SetSelectedOnMenu();
    }

    public void ShowCredits()
    {
        ShowCanvas(creditsGroup);
        mainMenuGroup.interactable = false;
        settingsGroup.interactable = false;
    }

    public void HideCredits()
    {
        HideCanvas(creditsGroup);
        settingsGroup.interactable = true;
    }

    public void ShowExit()
    {
        ShowCanvas(exitGroup);
        mainMenuGroup.interactable = false;
        deviceSwitch?.SetSelectedOnExit();
    }

    public void HideExit()
    {
        HideCanvas(exitGroup);
        mainMenuGroup.interactable = true;
        deviceSwitch?.SetSelectedOnMenu();
    }

    public void ShowSave()
    {
        HideSettings();
        ShowCanvas(saveDeleteGroup);
        mainMenuGroup.interactable = false;
        deviceSwitch?.SetSelectedOnExit();
    }

    public void HideSave()
    {
        HideCanvas(saveDeleteGroup);
        mainMenuGroup.interactable = true;
        deviceSwitch?.SetSelectedOnMenu();
    }

    public void HandleSettingsMainMenu()
    {
        HideExit();
        HideSettings();
        HideCredits();
        HideSave();
    }
}
