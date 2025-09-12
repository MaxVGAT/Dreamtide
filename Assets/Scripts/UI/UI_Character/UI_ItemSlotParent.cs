using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    [SerializeField] private UI ui;                       // assign in inspector
    [SerializeField] private Inventory_Player inventory;

    private UI_ItemSlot[] slots;

    private void Awake()
    {
        slots = GetComponentsInChildren<UI_ItemSlot>();
        foreach (var slot in slots)
        {
            slot.Setup(ui, inventory);
        }
    }

    public void UpdateSlots(List<Inventory_Item> itemList)
    {
        if (slots == null)
            slots = GetComponentsInChildren<UI_ItemSlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)
            {
                slots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                slots[i].UpdateSlot(null);
            }
        }
    }
}