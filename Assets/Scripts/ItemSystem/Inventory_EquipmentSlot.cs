using System;
using UnityEngine;

[Serializable]
public class Inventory_EquipmentSlot
{
    public Item_Type slotType;
    public Inventory_Item equippedItem;

    public Inventory_EquipmentSlot(Item_Type type)
    {
        slotType = type;
        equippedItem = null;
    }

    public Inventory_EquipmentSlot()
    {
        equippedItem = null;
    }

    public bool HasItem() => equippedItem != null && equippedItem.itemData != null;
}