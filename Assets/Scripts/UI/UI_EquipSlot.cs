using UnityEngine;
using UnityEngine.EventSystems;
using Unity.UI;
using UnityEngine.UI;

public class UI_EquipSlot : UI_ItemSlot
{
    public Inventory_Item equippedItem { get; private set; }

    [SerializeField] private Image equipIcon;

    public override void UpdateSlot(Inventory_Item item)
    {
        equippedItem = item;

        if (equippedItem == null)
        {
            if (itemIcon != null) itemIcon.color = Color.clear;
            return;
        }

        if (itemIcon != null && equippedItem.itemData != null)
        {
            itemIcon.sprite = equippedItem.itemData.itemIcon;
            Color color = Color.white;
            color.a = 0.9f;
            itemIcon.color = color;
        }
    }
}
