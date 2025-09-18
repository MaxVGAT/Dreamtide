using System;
using UnityEngine;

// 装備スロットを表すクラス
[Serializable]
public class Inventory_EquipmentSlot
{
    public Item_Type slotType;       // スロットの種類 (例: Helmet, Ring)
    public Inventory_Item equippedItem; // 装備中のアイテム

    // 指定スロットタイプでスロットを生成
    public Inventory_EquipmentSlot(Item_Type type)
    {
        slotType = type;
        equippedItem = null;
    }

    // デフォルトコンストラクタ
    public Inventory_EquipmentSlot()
    {
        equippedItem = null;
    }

    // スロットにアイテムが装備されているか
    public bool HasItem() => equippedItem != null && equippedItem.itemData != null;
}
