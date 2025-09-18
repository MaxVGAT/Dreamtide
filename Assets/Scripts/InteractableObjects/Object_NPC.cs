// NPCオブジェクトの基本クラス
using UnityEngine;

public class Object_NPC : MonoBehaviour
{
    protected Transform player;        // 接触プレイヤーのTransform
    protected UI ui;                   // UI参照

    [SerializeField] private Transform npc;               // NPC本体Transform
    [SerializeField] private GameObject interactTooltip;  // インタラクト用ツールチップ

    [Header("Tooltip Float details")]
    [SerializeField] private float floatSpeed = 2;        // ツールチップの上下速度
    [SerializeField] private float floatRange = 0.8f;    // ツールチップの上下幅
    private Vector3 startPosition;                        // ツールチップ初期位置

    // 初期化
    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();               // UIを取得
        startPosition = interactTooltip.transform.position;
        interactTooltip.SetActive(false);               // 初期は非表示
    }

    // 毎フレーム更新
    protected virtual void Update()
    {
        HandleTooltipFloat();
    }

    // ツールチップの浮遊処理
    private void HandleTooltipFloat()
    {
        if (interactTooltip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactTooltip.transform.position = startPosition + new Vector3(0, yOffset);
        }
    }

    // プレイヤー接触時
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        interactTooltip.SetActive(true);  // ツールチップ表示
    }

    // プレイヤー離脱時
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactTooltip.SetActive(false); // ツールチップ非表示
    }
}
