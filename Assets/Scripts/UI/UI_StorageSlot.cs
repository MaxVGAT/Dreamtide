using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Inventory_Storage storage;
    private UI_Storage uiStorage;
    public enum StorageSlotType { StorageSlot, PlayerInventorySlot }
    public StorageSlotType slotType;

    private void Start()
    {
        if (storage == null)
        {
            storage = FindAnyObjectByType<Inventory_Storage>();
        }
    }

    public void SetUIStorage(UI_Storage uiStorage)
    {
        this.uiStorage = uiStorage;
    }

    public void SetStorage(Inventory_Storage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (timeSinceLastClick < DoubleClickThreshold)
        {
            if (storage != null && uiStorage != null && uiStorage.storageRoot != null
                && uiStorage.storageRoot.activeInHierarchy)
            {
                if (slotType == StorageSlotType.StorageSlot)
                    storage.FromStorageToPlayer(itemInSlot);
                else if (slotType == StorageSlotType.PlayerInventorySlot)
                    storage.FromPlayerToStorage(itemInSlot);

                ui?.itemTooltip.ShowToolTip(false, null);
                return;
            }
            else
            {
                if (slotType == StorageSlotType.PlayerInventorySlot)
                {
                    inventory?.TryEquipItem(itemInSlot);
                }
            }
        }
    }
}