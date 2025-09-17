using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    [SerializeField] private UI ui;
    private Inventory_Player inventory;

    private UI_ItemSlot[] slots;

    private void Awake()
    {
        slots = GetComponentsInChildren<UI_ItemSlot>();
        // DON'T assign inventory yet
    }

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory_Player>();

        slots = GetComponentsInChildren<UI_ItemSlot>();
        foreach (var slot in slots)
            slot.Setup(ui, inventory);
    }

    private void OnEnable()
    {
        // Try to find the player inventory every time the UI becomes active
        if (inventory == null)
            inventory = FindAnyObjectByType<Inventory_Player>();

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
                slots[i].UpdateSlot(itemList[i]);
            else
                slots[i].UpdateSlot(null);
        }
    }
}

