using System;
using System.Collections.Generic;
using UnityEngine;

// �v���C���[��p�C���x���g���i�����Ǘ��܂ށj
public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed;

    private Entity_Player player; // �v���C���[�̃X�e�[�^�X�Q��
    public List<Inventory_EquipmentSlot> equipList; // �����X���b�g���X�g
    public Inventory_Storage storage{ get; private set; }

    [Header("Quick Item Slots")]
    public Inventory_Item[] quickItems = new Inventory_Item[2];

    [Header("Gold Infos")]
    public int gold = 10000;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>(); // �v���C���[�̃X�e�[�^�X�擾
    }

    public void SetQuickItemsInSlot(int slotNumber, Inventory_Item itemToSet)
    {
        quickItems[slotNumber - 1] = itemToSet;
        TriggerUpdateUI();
    }

    public void TryUseQuickItemInSlot(int passedSlotNumber)
    {
        int slotNumber = passedSlotNumber - 1;
        var itemToUse = quickItems[slotNumber];

        if (itemToUse == null)
            return;

        TryUseItem(itemToUse);

        if(FindItem(itemToUse) == null)
            quickItems[slotNumber] = FindSameItem(itemToUse);

        TriggerUpdateUI();
        OnQuickSlotUsed?.Invoke(slotNumber);
    }

    // �A�C�e���𑕔����悤�Ƃ���
    public void TryEquipItem(Inventory_Item item)
    {

        // --- Handle consumables first ---
        if (item.itemData.itemType == Item_Type.Consumables)
        {
            UseConsumable(item); // executes effect and reduces stack
            return; // stop here, do NOT touch equip slots
        }

        Inventory_Item inventoryItem = FindItem(item);
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // Add this check for consumables or items with no matching slots
        if (matchingSlots.Count == 0)
        {
            return;
        }

        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equippedItem;
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    public void TryUseItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item);

        if (inventoryItem == null)
            return;

        // Handle consumables
        if (inventoryItem.itemData.itemType == Item_Type.Consumables)
        {
            if (inventoryItem.itemEffect != null && inventoryItem.itemEffect.CanBeUsed(player))
                UseConsumable(inventoryItem);

            return; // don’t fall through to equip logic
        }

        // Otherwise, try equipping
        TryEquipItem(item);
    }


    private void UseConsumable(Inventory_Item consumable)
    {
        consumable.itemEffect.ExecuteEffect(player);
        RemoveOneItem(consumable);
    }

    // �w��X���b�g�ɃA�C�e���𑕔�
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.stats); // �X�e�[�^�X�ɏC���q��K�p
        slot.equippedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveOneItem(itemToEquip); // �C���x���g������폜
    }

    // �A�C�e���𑕔����
    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
        {
            Debug.Log("�C���x���g���ɋ󂫂�����܂���");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        // �����X���b�g����폜
        var slotToUnequip = equipList.Find(slot => slot.equippedItem == itemToUnequip);

        if (slotToUnequip != null)
            slotToUnequip.equippedItem = null;

        itemToUnequip.RemoveModifiers(player.stats); // �X�e�[�^�X�C���q��폜
        itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip); // �C���x���g���ɖ߂�
    }

    public override void SaveData(ref GameData data)
    {
        Debug.Log($"=== STARTING SAVE - itemList count: {itemList?.Count ?? 0} ===");

        data.gold = gold;
        data.inventory.Clear();
        data.equippedItems.Clear();

        // Save inventory items - simple approach using saveID
        int itemIndex = 0;
        foreach (var item in itemList)
        {
            Debug.Log($"Processing item {itemIndex}: {(item == null ? "NULL ITEM" : item.itemData?.itemName ?? "NULL ITEMDATA")}");

            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                Debug.Log($"  - saveID: '{saveID}', stackSize: {item.stackSize}");

                if (string.IsNullOrEmpty(saveID))
                {
                    Debug.LogError($"  - ERROR: Item {item.itemData.itemName} has null/empty saveID!");
                    itemIndex++;
                    continue;
                }

                // Simple approach: just use saveID as key
                if (data.inventory.ContainsKey(saveID) == false)
                    data.inventory[saveID] = 0;

                data.inventory[saveID] += item.stackSize;
                Debug.Log($"  - ✓ SAVED successfully with key: {saveID}");
            }
            else
            {
                Debug.LogWarning($"  - ✗ SKIPPED - item null: {item == null}, itemData null: {item?.itemData == null}");
            }
            itemIndex++;
        }

        // Save equipped items - also simple using saveID
        Debug.Log($"=== SAVING EQUIPPED ITEMS - equipList count: {equipList?.Count ?? 0} ===");

        foreach (var slot in equipList)
        {
            Debug.Log($"Checking slot type: {slot.slotType}, HasItem: {slot.HasItem()}");
            if (slot.HasItem())
            {
                string saveID = slot.equippedItem.itemData.saveID;

                Debug.Log($"  - Saving equipped item: {slot.equippedItem.itemData.itemName} in {slot.slotType} slot with saveID: {saveID}");
                data.equippedItems[saveID] = slot.slotType;
            }
        }

        Debug.Log($"Total inventory items saved: {data.inventory.Count}");
        Debug.Log($"Total equipped items saved: {data.equippedItems.Count}");
        Debug.Log($"=== SAVE COMPLETE ===");
    }

    public override void LoadData(GameData data)
    {
        Debug.Log($"=== STARTING LOAD ===");
        Debug.Log($"Loading inventory data - found {data.inventory.Count} item entries");

        gold = data.gold;

        // Clear existing equipment first to avoid stat conflicts
        Debug.Log("Clearing existing equipment modifiers...");
        foreach (var slot in equipList)
        {
            if (slot.HasItem())
            {
                Debug.Log($"Removing modifiers from {slot.equippedItem.itemData.itemName}");
                slot.equippedItem.RemoveModifiers(player.stats);
                slot.equippedItem.RemoveItemEffect();
                slot.equippedItem = null;
            }
        }

        // Load inventory items - simple approach
        foreach (var item in data.inventory)
        {
            string saveID = item.Key;
            int stackSize = item.Value;

            Debug.Log($"Loading item with saveID: {saveID}, stackSize: {stackSize}");

            // Get the item data from database
            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            if (itemData == null)
            {
                Debug.LogWarning("Item not found in database: " + saveID);
                continue;
            }

            Debug.Log($"Found item data: {itemData.itemName}");

            // Create items based on stack size
            for (int i = 0; i < stackSize; i++)
            {
                // Simple approach: create new item (will get new random stats if applicable)
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                Debug.Log($"  - Created item: {itemData.itemName}");

                AddItem(itemToLoad);
                Debug.Log($"  - Added {itemData.itemName} to inventory");
            }
        }

        // Load equipped items - simple approach
        Debug.Log($"Loading {data.equippedItems.Count} equipped items");

        foreach (var entry in data.equippedItems)
        {
            string saveID = entry.Key;
            Item_Type loadedSlotType = entry.Value;

            Debug.Log($"Loading equipped item with saveID: {saveID}, slot type: {loadedSlotType}");

            // Get item data
            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            if (itemData == null)
            {
                Debug.LogError($"Could not find item data for equipped item: {saveID}");
                continue;
            }

            // Create the equipped item (will get new random stats)
            Inventory_Item itemToLoad = new Inventory_Item(itemData);
            Debug.Log($"  - Created equipped item: {itemData.itemName}");

            // Find empty slot of matching type
            var slot = equipList.Find(slot => slot.slotType == loadedSlotType && slot.HasItem() == false);

            if (slot == null)
            {
                Debug.LogError($"Could not find empty slot for item type: {loadedSlotType}");
                continue;
            }

            // Equip the item and apply its effects
            Debug.Log($"  - Equipping {itemData.itemName} to {loadedSlotType} slot");
            slot.equippedItem = itemToLoad;
            slot.equippedItem.AddModifiers(player.stats);
            slot.equippedItem.AddItemEffect(player);
        }

        Debug.Log($"Finished loading. Final itemList count: {itemList.Count}");
        Debug.Log($"=== LOAD COMPLETE ===");
        TriggerUpdateUI();
    }
}
