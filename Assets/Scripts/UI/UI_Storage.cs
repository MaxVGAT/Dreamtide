using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Storage storage;
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    public GameObject storageRoot;

    public void SetupStorage(Inventory_Player inventory, Inventory_Storage storage)
    {
        this.inventory = inventory;
        this.storage = storage;

        // Subscribe to inventory changes
        storage.OnInventoryChange += UpdateUI;

        // Update UI immediately
        UpdateUI();

        // Setup storage slots properly by type
        SetupStorageSlots();
    }

    private void SetupStorageSlots()
    {
        Debug.Log("Setting up storage slots...");

        // Get all storage slots in the UI
        UI_StorageSlot[] allSlots = GetComponentsInChildren<UI_StorageSlot>();
        Debug.Log($"Found {allSlots.Length} total slots");

        foreach (var slot in allSlots)
        {
            // Set references for all slots
            slot.SetStorage(storage);
            slot.SetUIStorage(this);

            // Log for debugging
            Debug.Log($"Setup slot: {slot.name} as {slot.slotType}");
        }

        // Alternative approach: Setup slots by parent
        SetupSlotsByParent();
    }

    private void SetupSlotsByParent()
    {
        // Setup storage slots (under storageParent)
        if (storageParent != null)
        {
            UI_StorageSlot[] storageSlots = storageParent.GetComponentsInChildren<UI_StorageSlot>();
            foreach (var slot in storageSlots)
            {
                slot.slotType = UI_StorageSlot.StorageSlotType.StorageSlot;
                slot.SetStorage(storage);
                slot.SetUIStorage(this);
                Debug.Log($"Setup STORAGE slot: {slot.name}");
            }
        }

        // Setup player inventory slots (under inventoryParent)  
        if (inventoryParent != null)
        {
            UI_StorageSlot[] inventorySlots = inventoryParent.GetComponentsInChildren<UI_StorageSlot>();
            foreach (var slot in inventorySlots)
            {
                slot.slotType = UI_StorageSlot.StorageSlotType.PlayerInventorySlot;
                slot.SetStorage(storage);
                slot.SetUIStorage(this);
                Debug.Log($"Setup INVENTORY slot: {slot.name}");
            }
        }
    }

    private void UpdateUI()
    {
        if (inventory != null && inventoryParent != null)
        {
            inventoryParent.UpdateSlots(inventory.itemList);
        }

        if (storage != null && storageParent != null)
        {
            storageParent.UpdateSlots(storage.itemList);
        }

        // Re-establish slot references after UI update
        SetupStorageSlots();
    }

    // Clean up when storage UI is closed
    private void OnDisable()
    {
        if (storage != null)
        {
            storage.OnInventoryChange -= UpdateUI;
        }
    }

    // Optional: Method to refresh slot setup if needed
    public void RefreshSlotSetup()
    {
        SetupStorageSlots();
    }
}