using UnityEngine;
using UnityEngine.UI;

public class UI_TreeConnections : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;         // 線の回転の基点
    [SerializeField] private RectTransform connectLength;         // 線の長さを制御
    [SerializeField] private RectTransform childNodeConnectionPoint; // 子ノード接続位置

    // 線を方向・長さ・オフセットで設定
    public void DirectConnection(NodeDirectionType direction, float length, float offset)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;
        float finalLength = shouldBeActive ? length : 0;
        float angle = GetDirectionAngle(direction);

        rotationPoint.localRotation = Quaternion.Euler(0, 0, angle + offset);
        connectLength.sizeDelta = new Vector2(finalLength, connectLength.sizeDelta.y);
    }

    // 線のImageを取得（色変更などに使用）
    public Image GetConnectionImage() => connectLength.GetComponent<Image>();

    // 子ノード接続位置を取得（親のRectTransform基準の座標）
    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
            rect.parent as RectTransform,
            childNodeConnectionPoint.position,
            null,
            out var localPosition
            );

        return localPosition;
    }

    // NodeDirectionTypeに応じた角度を返す
    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch (type)
        {
            case NodeDirectionType.UpLeft: return 135f;
            case NodeDirectionType.Up: return 90f;
            case NodeDirectionType.UpRight: return 45f;
            case NodeDirectionType.Left: return 180f;
            case NodeDirectionType.Right: return 0f;
            case NodeDirectionType.DownLeft: return -135f;
            case NodeDirectionType.Down: return -90f;
            case NodeDirectionType.DownRight: return -45f;
            default: return 0f;
        }
    }
}

// 接続方向の列挙
public enum NodeDirectionType
{
    None,
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}
