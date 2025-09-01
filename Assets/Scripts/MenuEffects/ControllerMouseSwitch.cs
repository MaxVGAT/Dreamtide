using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// コントローラーとマウス入力を切り替え、UIの選択状態を管理
public class ControllerMouseSwitch : MonoBehaviour
{
    [Header("UI選択オブジェクト")]
    public GameObject firstSelected;    // メニュー初期選択
    public GameObject settingsSelected; // 設定画面選択
    public GameObject exitSelected;     // 終了確認画面選択

    [Header("入力アクション")]
    public InputActionReference closeWindow; // ウィンドウ閉じる入力

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

    // 入力デバイス変更を監視（マウスかコントローラーか）
    private void OnInputActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;

        InputAction inputAction = (InputAction)obj;
        InputControl lastControl = inputAction.activeControl;
        InputDevice lastDevice = lastControl.device;

        Cursor.visible = lastDevice.displayName == "Mouse"; // マウスならカーソル表示
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

    // ウィンドウ閉じる操作
    public void OnCloseWindow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // 設定・クレジット・終了画面を閉じる処理
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

    // メニュー、設定、終了画面でのUI選択状態設定
    public void SetSelectedOnMenu() => EventSystem.current.SetSelectedGameObject(firstSelected);
    public void SetSelectedOnSettings()
    {
        Debug.Log("SetSelectedOnSettings called!");
        EventSystem.current.SetSelectedGameObject(settingsSelected);
    }
    public void SetSelectedOnExit() => EventSystem.current.SetSelectedGameObject(exitSelected);
}
