using System;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    public Item_DataSO itemData;

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
    }
}
