using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// �R���g���[���[�ƃ}�E�X���͂�؂�ւ��AUI�̑I���Ԃ�Ǘ�
public class ControllerMouseSwitch : MonoBehaviour
{
    [Header("UI�I��I�u�W�F�N�g")]
    public GameObject firstSelected;    // ���j���[�����I��
    public GameObject settingsSelected; // �ݒ��ʑI��
    public GameObject exitSelected;     // �I���m�F��ʑI��

    [Header("���̓A�N�V����")]
    public InputActionReference closeWindow; // �E�B���h�E�������

    private void Start()
    {
        SetSelectedOnMenu();
        Cursor.visible = true;

        if (closeWindow != null)
        {
            closeWindow.action.Enable();
            closeWindow.action.performed += OnCloseWindow;
        }
    }

    // ���̓f�o�C�X�ύX��Ď��i�}�E�X���R���g���[���[���j
    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;

        InputAction inputAction = (InputAction)obj;
        InputControl lastControl = inputAction.activeControl;
        InputDevice lastDevice = lastControl.device;

        Cursor.visible = lastDevice.displayName == "Mouse"; // �}�E�X�Ȃ�J�[�\���\��
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

    // �E�B���h�E���鑀��
    public void OnCloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // �ݒ�E�N���W�b�g�E�I����ʂ���鏈��
        if (ShowHideSettings.Instance != null)
        {
            var shs = ShowHideSettings.Instance;

            if (shs.settingsGroup != null && shs.settingsGroup.alpha > 0)
            {
                shs.HideSettings();
                SoundManager.Instance.PlayCloseButtonSFX();
            }

            if (shs.creditsGroup != null && shs.creditsGroup.alpha > 0)
            {
                shs.HideCredits();
                SoundManager.Instance.PlayCloseButtonSFX();
                shs.ShowSettings();
            }

            if (shs.exitGroup != null && shs.exitGroup.alpha > 0)
            {
                shs.HideExit();
                SoundManager.Instance.PlayCloseButtonSFX();
            }
        }
    }

    // ���j���[�A�ݒ�A�I����ʂł�UI�I���Ԑݒ�
    public void SetSelectedOnMenu() => EventSystem.current.SetSelectedGameObject(firstSelected);
    public void SetSelectedOnSettings()
    {
        Debug.Log("SetSelectedOnSettings called!");
        EventSystem.current.SetSelectedGameObject(settingsSelected);
    }
    public void SetSelectedOnExit() => EventSystem.current.SetSelectedGameObject(exitSelected);
}
