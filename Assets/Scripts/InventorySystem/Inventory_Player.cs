using System.Collections.Generic;
using UnityEngine;

// プレイヤー専用インベントリ（装備管理含む）
public class Inventory_Player : Inventory_Base
{
    private Entity_Stats playerStats; // プレイヤーのステータス参照
    public List<Inventory_EquipmentSlot> equipList; // 装備スロットリスト

    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponent<Entity_Stats>(); // プレイヤーのステータス取得
    }

    // アイテムを装備しようとする
    public void TryEquipItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item.itemData); // インベントリ内のアイテム取得
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // 空きスロットを探して装備
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        // 空きがなければ最初のスロットのアイテムと入れ替え
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equippedItem;

        EquipItem(inventoryItem, slotToReplace);
        UnequipItem(itemToUnequip);
    }

    // 指定スロットにアイテムを装備
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(playerStats); // ステータスに修飾子を適用

        RemoveItem(itemToEquip); // インベントリから削除
    }

    // アイテムを装備解除
    public void UnequipItem(Inventory_Item itemToUnequip)
    {
        if (CanAddItem() == false)
        {
            Debug.Log("インベントリに空きがありません");
            return;
        }

        // 装備スロットから削除
        foreach (var slot in equipList)
        {
            if (slot.equippedItem == itemToUnequip)
            {
                slot.equippedItem = null;
                break;
            }
        }

        itemToUnequip.RemoveModifiers(playerStats); // ステータス修飾子を削除
        AddItem(itemToUnequip); // インベントリに戻す
    }
}
