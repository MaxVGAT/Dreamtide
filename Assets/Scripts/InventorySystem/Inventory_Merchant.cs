using System.Collections.Generic;
using UnityEngine;

public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player inventory;

    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemsAmount = 4;
    [SerializeField] private int maxItemsAmount = 10;

    public void FillShopList()
    {
        itemList.Clear();
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();
        
        foreach(var itemData in shopData.itemList)
        {
            int randomizedStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomizedStack, 1, itemData.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStack;

            possibleItems.Add(itemToAdd);
        }

        int randomItemAmount = Random.Range(minItemsAmount, maxItemsAmount + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);
    }

    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
