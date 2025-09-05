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

        bool transferAll = Input.GetKey(KeyCode.LeftControl);

        if (timeSinceLastClick < DoubleClickThreshold)
        {
            if (storage != null && uiStorage != null && uiStorage.storageRoot != null
                && uiStorage.storageRoot.activeInHierarchy)
            {
                if (slotType == StorageSlotType.StorageSlot)
                    storage.FromStorageToPlayer(itemInSlot, transferAll);
                else if (slotType == StorageSlotType.PlayerInventorySlot)
                    storage.FromPlayerToStorage(itemInSlot, transferAll);

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