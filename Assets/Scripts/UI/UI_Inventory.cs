using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Base inventory;
    private UI_ItemSlot[] uiItemSlots;
    private UI_EquipSlot[] uiEquipSlots;

    [SerializeField] private Transform uiItemSlotParent;
    [SerializeField] private Transform uiEquipSlotParent;

    private void Awake()
    {
        uiItemSlots = uiItemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        uiEquipSlots = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Base>();
        inventory.OnInventoryChange += UpdateInventorySlots;

        UpdateInventorySlots();
    }

    private void UpdateEquipmentSlots()
    {

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
