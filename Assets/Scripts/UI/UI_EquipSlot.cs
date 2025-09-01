using UnityEngine;
using UnityEngine.EventSystems;

// 装備スロットUIを管理するクラス
// UI_ItemSlot を継承して、装備可能なアイテムタイプを指定できる
public class UI_EquipSlot : UI_ItemSlot
{
    public Item_Type slotType; // このスロットで装備可能なアイテムタイプ

    // エディタ上でスロット名を自動更新する
    private void OnValidate()
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();
    }

    // スロットがクリックされた時の処理
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) // スロットにアイテムがなければ何もしない
            return;

        // ダブルクリック判定
        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.UnequipItem(itemInSlot); // アイテムを外す
            lastClickTime = 0;                 // クリックタイマーをリセット
        }
        else
            lastClickTime = Time.time;         // クリック時間を更新
    }
}
