using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerMouseSwitch : MonoBehaviour
{
    public GameObject firstSelected;

    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(firstSelected);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        
    }

    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if(change == InputActionChange.ActionPerformed)
        {
            InputAction inputAction = (InputAction)obj;
            InputControl lastControl = inputAction.activeControl;
            InputDevice lastDevice = lastControl.device;

             if(lastDevice.displayName == "Mouse")
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
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnInputActionChange;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(Settings)
        }
    }
}
