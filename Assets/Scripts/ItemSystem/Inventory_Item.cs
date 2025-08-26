using System;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    public Item_DataSO itemData;
    public int stackSize = 1;

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
    }
    
    public bool CanAddStack() => stackSize < itemData.maxStackSize;
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
