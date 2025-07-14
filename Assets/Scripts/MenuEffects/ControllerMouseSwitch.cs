using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerMouseSwitch : MonoBehaviour
{
    public GameObject firstSelected;
    public GameObject settingsSelected;
    public GameObject exitSelected;
    public InputActionReference closeWindow;

    void Start()
    {
        SetSelectedOnMenu();

        Cursor.visible = true;

        if (closeWindow != null)
        {
            closeWindow.action.Enable();
            closeWindow.action.performed += OnCloseWindow;
        }
    }

    private void Update()
    {

    }

    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction inputAction = (InputAction)obj;
            InputControl lastControl = inputAction.activeControl;
            InputDevice lastDevice = lastControl.device;

            if (lastDevice.displayName == "Mouse")
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
        }
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnInputActionChange;
        if (closeWindow != null)
        {
            closeWindow.action.Enable();
            closeWindow.action.performed += OnCloseWindow;
        }
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnInputActionChange;
        if (closeWindow != null)
        {
            closeWindow.action.performed -= OnCloseWindow;
            closeWindow.action.Disable();
        }
    }

    public void OnCloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pressed escape");
        }

        if (ShowHideSettings.Instance != null)
        {
            if (ShowHideSettings.Instance.settingsGroup != null && ShowHideSettings.Instance.settingsGroup.alpha > 0)
            {
                ShowHideSettings.Instance.HideSettings();
                SoundManager.Instance.PlayCloseButtonSFX();
            }

            if (ShowHideSettings.Instance.creditsGroup != null && ShowHideSettings.Instance.creditsGroup.alpha > 0)
            {
                ShowHideSettings.Instance.HideCredits();
                SoundManager.Instance.PlayCloseButtonSFX();
                ShowHideSettings.Instance.ShowSettings();
            }

            if (ShowHideSettings.Instance.exitGroup != null && ShowHideSettings.Instance.exitGroup.alpha > 0)
            {
                ShowHideSettings.Instance.HideExit();
                SoundManager.Instance.PlayCloseButtonSFX();
            }
        }
    }

    public void SetSelectedOnMenu()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SetSelectedOnSettings()
    {
        Debug.Log("SetSelectedOnSettings called!");
        EventSystem.current.SetSelectedGameObject(settingsSelected);
    }

    public void SetSelectedOnExit()
    {
        EventSystem.current.SetSelectedGameObject(exitSelected);
    }
}