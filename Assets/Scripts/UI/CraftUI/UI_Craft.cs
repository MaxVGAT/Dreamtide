using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;

    private Inventory_Player inventory;
    private UI_CraftPreview craftPreviewUI;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButtons;

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreviewUI = GetComponentInChildren<UI_CraftPreview>(true);
        craftPreviewUI.SetupCraftPreview(storage);

        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
        // Get all slots and buttons in children automatically
        craftSlots = GetComponentsInChildren<UI_CraftSlot>(true);
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>(true);

        // Hide all slots initially
        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        // Give every button a reference to the slots
        foreach (var button in craftListButtons)
            button.SetCraftSlot(craftSlots, craftPreviewUI); // pass preview so buttons can auto-update
    }

    private void UpdateUI() => inventoryParent.UpdateSlots(inventory.itemList);
}
