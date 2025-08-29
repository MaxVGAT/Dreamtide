using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    private Entity_Stats playerStats;
    public List<Inventory_EquipmentSlot> equipList;

    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponent<Entity_Stats>();

        equipList = new List<Inventory_EquipmentSlot>()
    {
        new Inventory_EquipmentSlot(Item_Type.Weapon),
        new Inventory_EquipmentSlot(Item_Type.Trinket),
        new Inventory_EquipmentSlot(Item_Type.Trinket),
        new Inventory_EquipmentSlot(Item_Type.Trinket)
        // Adjust ItemType enums to match your actual types
    };

    }

    public void TryEquipItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item.itemData);
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        //Step 1: Try to find empty slot and equip item
        foreach(var slot in matchingSlots)
        {
            if(slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(playerStats);

        RemoveItem(itemToEquip);
    }

    public void UnequipItem(Inventory_Item itemToUnequip)
    {
        if(CanAddItem() == false)
        {
            Debug.Log("No space");
            return;
        }

        foreach(var slot in equipList)
        {
            if(slot.equippedItem == itemToUnequip)
            {
                slot.equippedItem.RemoveModifiers(playerStats);
                slot.equippedItem = null;
                AddItem(itemToUnequip);
                break;
            }
        }
    }
}
