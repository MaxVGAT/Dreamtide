using UnityEngine;

// プレイヤーのステータスUIを管理するクラス
public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] uiStatSlots; // 各ステータスを表示するUIスロット
    private Inventory_Player inventory; // プレイヤーのインベントリ/ステータス管理

    private void Awake()
    {
        // 子オブジェクトから全てのステータススロットを取得
        uiStatSlots = GetComponentsInChildren<UI_StatSlot>();

        // シーン上の最初のInventory_Playerを取得
        inventory = FindFirstObjectByType<Inventory_Player>();

        // インベントリが更新されたときにUIを更新するイベントに登録
        inventory.OnInventoryChange += UpdateStatsUI;
    }

    private void Start()
    {
        // 初期表示の更新
        UpdateStatsUI();
    }

    // 各UIスロットのステータス値を更新
    private void UpdateStatsUI()
    {
        foreach (var statSlot in uiStatSlots)
        {
            statSlot.UpdateStatValue();
        }
    }
}
