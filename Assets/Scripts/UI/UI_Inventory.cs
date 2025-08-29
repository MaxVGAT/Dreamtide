using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;
    private UI_ItemSlot[] uiItemSlots;
    private UI_EquipSlot[] uiWeaponTrinketSlots;
    private UI_EquipSlot[] uiArmorSlots;

    [SerializeField] private Transform uiItemSlotParent;
    [SerializeField] private Transform uiWeaponTrinketParent;
    [SerializeField] private Transform uiArmorSlotParent;

    private void Awake()
    {
        uiItemSlots = uiItemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        uiWeaponTrinketSlots = uiWeaponTrinketParent.GetComponentsInChildren<UI_EquipSlot>();
        uiArmorSlots = uiArmorSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateInventorySlots();
        UpdateEquipmentSlots();
    }

    private void UpdateEquipmentSlots()
    {
        for(int i = 0; i < uiWeaponTrinketSlots.Length; i++)
        {
            if (i < inventory.equipList.Count)
            {
                var slot = inventory.equipList[i];
                uiWeaponTrinketSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }

        for(int i = 0; i < uiArmorSlots.Length; i++)
        {
            int equipListIndex = i + 4;
            if(equipListIndex  < inventory.equipList.Count)
            {
                var slot = inventory.equipList[equipListIndex];
                uiArmorSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }
    }

    private void UpdateInventorySlots()
    {
        List<Inventory_Item> itemList = inventory.itemList;

        for(int i = 0; i < uiItemSlots.Length; i++)
        {
            if(i < itemList.Count)
            {
                uiItemSlots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                uiItemSlots[i].UpdateSlot(null);
            }
        }
    }

    public void RefreshInventoryUI()
    {
        UpdateInventorySlots();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
