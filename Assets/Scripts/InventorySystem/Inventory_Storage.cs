using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory {  get; private set; }

    public bool CanCraftItems(Inventory_Item itemToCraft)
    {
        return hasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }

    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);
    }

    private void ConsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach(var requiredItem in itemToCraft.itemData.craftRecipe)
        {
            int amountToConsume = requiredItem.stackSize;

            amountToConsume -= ConsumedMaterialsAmount(playerInventory.itemList, requiredItem);

            if (amountToConsume > 0)
                amountToConsume -= ConsumedMaterialsAmount(itemList, requiredItem);
        }
    }

    private int ConsumedMaterialsAmount(List<Inventory_Item> itemList, Inventory_Item neededItem)
    {
        int amountNeeded = neededItem.stackSize;

        int consumedAmount = 0;

        foreach(var item in itemList)
        {
            if (item.itemData != neededItem.itemData)
                continue;

            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - consumedAmount);
            item.stackSize -= removeAmount;
            consumedAmount += removeAmount;

            if (item.stackSize <= 0)
                itemList.Remove(item);

            if (consumedAmount >= amountNeeded)
                break;
        }

        return consumedAmount;
    }

    private bool hasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach(var requiredMaterial in itemToCraft.itemData.craftRecipe)
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.stackSize)
                return false;
        }

        return true;
    }

    public int GetAvailableAmountOf(Item_DataSO requiredItem)
    {
        int amount = 0;

        foreach(var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach(var item in itemList)
        {
            if(item.itemData == requiredItem)
                amount += item.stackSize;
        }

        return amount;
    }

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;

    public void FromPlayerToStorage(Inventory_Item item, bool transferAll)
    {
        int transferAmount = transferAll ? item.stackSize : 1;

        for(int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);
                playerInventory.RemoveOneItem(item);
                AddItem(itemToAdd);
            }
        }

        SortItems();
        TriggerUpdateUI();
    }

    public void SortItems()
    {
        itemList = itemList.OrderBy(item => item.itemData.name).ThenBy(item => item.stackSize).ToList();
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferAll)
    {
        int transferAmount = transferAll ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);
                RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);
            }
        }
        SortItems();
        TriggerUpdateUI();
    }

    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        data.storageItems.Clear();

        foreach(var entry in itemList)
        {
            if(entry != null && entry.itemData != null)
            {
                string saveID = entry.itemData.saveID;

                if (data.storageItems.ContainsKey(saveID) == false)
                    data.storageItems[saveID] = 0;

                data.storageItems[saveID] += entry.stackSize;
            }
        }
    }

    public override void LoadData(GameData data)
    {
        itemList.Clear();

        foreach(var entry in data.storageItems)
        {
            string saveID = entry.Key;
            int stackSize = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);

            if(itemData == null)
            {
                Debug.LogWarning("Item not found: " + saveID);
                continue;
            }

            Inventory_Item itemToLoad = new Inventory_Item(itemData);

            for(int i = 0; i < stackSize; i++)
            {
            itemList.Add(itemToLoad);
            }

        }
    }
}
