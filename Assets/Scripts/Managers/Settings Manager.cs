using UnityEngine;

public class ShowHideSettings : MonoBehaviour
{
    [Header("CanvasGroups")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup settingsGroup;
    public CanvasGroup exitGroup;
    public CanvasGroup saveDeleteGroup;

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
        if (exitGroup == null)
            exitGroup = GameObject.Find("ExitGroup")?.GetComponent<CanvasGroup>();
        if (saveDeleteGroup == null)
            saveDeleteGroup = GameObject.Find("SaveDeleteGroup")?.GetComponent<CanvasGroup>();
    }

    private void HideAllPanelsImmediate()
    {
        HideCanvas(settingsGroup);
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
    }

    public void HideSettings()
    {
        HideCanvas(settingsGroup);
        mainMenuGroup.interactable = true;
    }

    public void ShowExit()
    {
        ShowCanvas(exitGroup);
        mainMenuGroup.interactable = false;
    }

    public void HideExit()
    {
        HideCanvas(exitGroup);
        mainMenuGroup.interactable = true;
    }

    public void ShowSave()
    {
        HideSettings();
        ShowCanvas(saveDeleteGroup);
        mainMenuGroup.interactable = false;
    }

    public void HideSave()
    {
        HideCanvas(saveDeleteGroup);
        mainMenuGroup.interactable = true;
    }

    public void HandleSettingsMainMenu()
    {
        HideExit();
        HideSettings();
        HideSave();
    }
}
