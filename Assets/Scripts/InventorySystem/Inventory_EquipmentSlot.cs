using System;
using UnityEngine;

// 装備スロットのデータ管理クラス
[Serializable]
public class Inventory_EquipmentSlot
{
    public Item_Type slotType;       // このスロットの種類（例：Helmet, Ring）
    public Inventory_Item equippedItem; // 装備中のアイテム

    // 指定タイプでスロットを作成
    public Inventory_EquipmentSlot(Item_Type type)
    {
        slotType = type;
        equippedItem = null;
    }

    // デフォルトコンストラクタ（装備なし）
    public Inventory_EquipmentSlot()
    {
        equippedItem = null;
    }

    // スロットに装備中のアイテムがあるか
    public bool HasItem() => equippedItem != null && equippedItem.itemData != null;
}
