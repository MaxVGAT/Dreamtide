using UnityEngine;
using System.Collections.Generic;

public class UI_EquipSlotParent : MonoBehaviour
{
    [SerializeField] private Transform weaponTrinketParent;
    [SerializeField] private Transform armorParent;

    private UI_EquipSlot[] weaponTrinketSlots;
    private UI_EquipSlot[] armorSlots;

    public void UpdateEquipmentSlots(List<Inventory_EquipmentSlot> equipList)
    {
        if (weaponTrinketSlots == null)
            weaponTrinketSlots = weaponTrinketParent.GetComponentsInChildren<UI_EquipSlot>();

        if (armorSlots == null)
            armorSlots = armorSlots = armorParent.GetComponentsInChildren<UI_EquipSlot>();

        // Weapons / Trinkets (first 4 slots in equipList)
        for (int i = 0; i < weaponTrinketSlots.Length; i++)
        {
            if (i < equipList.Count)
                weaponTrinketSlots[i].UpdateSlot(equipList[i].HasItem() ? equipList[i].equippedItem : null);
        }

        // Armor (offset by 4 in equipList)
        for (int i = 0; i < armorSlots.Length; i++)
        {
            int equipListIndex = i + 4;
            if (equipListIndex < equipList.Count)
                armorSlots[i].UpdateSlot(equipList[equipListIndex].HasItem() ? equipList[equipListIndex].equippedItem : null);
        }
    }
}
