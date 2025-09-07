using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory {  get; private set; }

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

        TriggerUpdateUI();
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

        TriggerUpdateUI();
    }
}
