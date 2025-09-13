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
        data.gold = gold;
        data.inventory.Clear();
        data.equippedItems.Clear();

        foreach(var item in itemList)
        {
            if(item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.inventory.ContainsKey(saveID) == false)
                    data.inventory[saveID] = 0;

                data.inventory[saveID] += item.stackSize;
            }
        }

        foreach(var slot in equipList)
        {
            if (slot.HasItem())
                data.equippedItems[slot.equippedItem.itemData.saveID] = slot.slotType;
        }
    }

    public override void LoadData(GameData data)
    {
        gold = data.gold;

        foreach(var item in data.inventory)
        {
            string saveID = item.Key;
            int stackSize = item.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);

            if(itemData == null)
            {
                Debug.LogWarning("Item not found: " + saveID);
                continue;
            }

            for(int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach(var entry in data.equippedItems)
        {
            string saveID = entry.Key;
            Item_Type loadedSlotType = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            Inventory_Item itemToLoad = new Inventory_Item(itemData);

            var slot = equipList.Find(slot => slot.slotType == loadedSlotType && slot.HasItem() == false);

            slot.equippedItem = itemToLoad;
            slot.equippedItem.AddModifiers(player.stats);
            slot.equippedItem.AddItemEffect(player);
        }

        TriggerUpdateUI();
    }
}
