// シーン遷移用ウェイポイント
using UnityEngine;

public class Object_Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;       // 遷移先シーン名
    [Space]
    [SerializeField] private Respawn_Type waypointType;   // このウェイポイントのタイプ
    [SerializeField] private Respawn_Type conntedWaypoint;// 接続されるウェイポイントのタイプ
    [SerializeField] private Transform respwanPoint;      // プレイヤーの復帰位置
    [SerializeField] private bool canBeTriggered = true;  // 発動可能か

    // ウェイポイントタイプ取得
    public Respawn_Type GetWaypointType() => waypointType;

    // ポジション取得とトリガー無効化
    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;
        return respwanPoint == null ? transform.position : respwanPoint.position;
    }

    // トリガー状態設定
    public void SetTriggerState(bool state)
    {
        canBeTriggered = state;
    }

    // Inspectorで設定を自動調整
    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToScene;

        if (waypointType == Respawn_Type.Enter)
            conntedWaypoint = Respawn_Type.Exit;

        if (waypointType == Respawn_Type.Exit)
            conntedWaypoint = Respawn_Type.Enter;
    }

    // プレイヤー接触時にシーン変更
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;

        GameManager.instance.ChangeScene(transferToScene, conntedWaypoint);
    }

    // プレイヤー離脱時にトリガー再有効化
    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }
}
