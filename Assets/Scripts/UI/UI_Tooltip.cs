using UnityEngine;

// ツールチップ表示用クラス
public class UI_Tooltip : MonoBehaviour
{
    protected RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300, 20); // ツールチップの表示オフセット

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
        // 初期位置を画面外に設定
        rect.position = new Vector2(9999, 9999);
    }

    // ツールチップ表示/非表示
    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (!show)
        {
            rect.position = new Vector2(9999, 9999); // 非表示時は画面外へ
            return;
        }

        UpdatePosition(targetRect);
    }

    // ツールチップの位置更新
    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;

        Vector2 targetPosition = targetRect.position;

        // 画面の左右でオフセット反転
        targetPosition.x = targetPosition.x > screenCenterX
            ? targetPosition.x - offset.x
            : targetPosition.x + offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        // 画面上端/下端にはみ出さないように補正
        if (topY > screenTop)
            targetPosition.y = screenTop - verticalHalf - offset.y;
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom + verticalHalf + offset.y;

        rect.position = targetPosition;
    }

    // 文字列に色付け
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
