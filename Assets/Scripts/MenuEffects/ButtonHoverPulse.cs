using UnityEngine;
using UnityEngine.EventSystems;

// ボタンホバー時に拡大・縮小のパルスアニメーションとSFXを再生
public class ButtonHoverPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Hover Settings")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f); // ホバー時の拡大サイズ
    [SerializeField] private float pulseSpeed = 1f; // パルス速度

    [Header("Audio")]
    [SerializeField] private AudioSource SFXSource; // SFX再生用

    private Vector3 originalScale; // 元のサイズ
    private bool isHovering = false; // ホバー中かどうか

    private void Start()
    {
        originalScale = transform.localScale; // 初期サイズ取得
    }

    private void Update()
    {
        // ホバー中なら拡大、そうでなければ元のサイズに補間
        Vector3 targetScale = isHovering ? Vector3.Scale(originalScale, hoverScale) : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * pulseSpeed);
    }

    // マウスがボタン上に入ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        EventSystem.current.SetSelectedGameObject(gameObject); // 選択状態に設定

        // ホバーSFX再生
        if (SFXSource != null)
            SoundManager.Instance?.PlayHoverSFX();
    }

    // マウスがボタン上から出たとき
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    // UI選択時（キーボードやコントローラー操作）
    public void OnSelect(BaseEventData eventData)
    {
        isHovering = true;

        if (SFXSource != null)
            SoundManager.Instance?.PlayHoverSFX();
    }

    // UI選択解除時
    public void OnDeselect(BaseEventData eventData)
    {
        isHovering = false;

        // 選択解除時にポインター退出イベントを送信
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointerData, ExecuteEvents.pointerExitHandler);
    }
}
