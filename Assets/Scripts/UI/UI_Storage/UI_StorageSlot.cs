using UnityEngine;
using UnityEngine.EventSystems;
public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;
    private Inventory_Merchant merchant;
    private UI_Storage uiStorage;
    public enum StorageSlotType { StorageSlot, PlayerInventorySlot }
    public StorageSlotType slotType;
    private void Start()
    {
        if (storage == null)
        {
            storage = FindAnyObjectByType<Inventory_Storage>();
        }
        if (merchant == null)
        {
            merchant = FindAnyObjectByType<Inventory_Merchant>();
        }
    }
    public void SetUIStorage(UI_Storage uiStorage) => this.uiStorage = uiStorage;
    public void SetStorage(Inventory_Storage storage) => this.storage = storage;
    public void SetMerchant(Inventory_Merchant merchant) => this.merchant = merchant;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        bool rightButton = eventData.button == PointerEventData.InputButton.Right;
        float timeSinceLastClick = Time.time - lastClickTime;
        
        if(!rightButton) lastClickTime = Time.time;

        // Transfer items to storage
        if (!rightButton && timeSinceLastClick < DoubleClickThreshold)
        {
            HandleDoubleClick();
            return;
        }

        // Sell items to shop
        if (rightButton)
        {
            HandleRightClick();
        }
    }

    private void HandleDoubleClick()
    {
        // Only handle double-click outside storage, e.g., equip item
        if (storage == null || ui == null || !ui.IsStorageVisible())
        {
            if (slotType == StorageSlotType.PlayerInventorySlot)
            {
                inventory?.TryEquipItem(itemInSlot);
            }
        }
        // Otherwise do nothing, storage transfers are handled by right-click
    }

    private void HandleRightClick()
    {
        bool transferAll = Input.GetKey(KeyCode.LeftControl);

        if(storage != null && ui != null && ui.IsStorageVisible())
        {
            if(slotType == StorageSlotType.PlayerInventorySlot)
            {
                storage.FromPlayerToStorage(itemInSlot, transferAll);
            }
            else if(slotType == StorageSlotType.StorageSlot)
            {
                storage.FromStorageToPlayer(itemInSlot, transferAll);
            }
            ui?.itemTooltip.ShowToolTip(false, null);
            return;
        }

        // Check if merchant/shop UI is visible
        if (merchant != null && ui != null && ui.IsMerchantVisible())
        {
            // Shop is open - sell the item
            if (merchant.inventory == null)
            {
                merchant.SetInventory(inventory);
            }
            merchant.TrySellItem(itemInSlot, transferAll);
        }
        else
        {
            // Shop is not open - delete the item from inventory
            if (transferAll)
            {
                // Remove entire stack
                inventory?.RemoveAllItems(itemInSlot);
            }
            else
            {
                // Remove one item
                inventory?.RemoveOneItem(itemInSlot);
            }
        }

        ui?.itemTooltip?.ShowToolTip(false, null);
    }
}