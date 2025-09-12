using UnityEngine;
using System.Collections.Generic;

public class Player_DropManager : Entity_DropManager
{
    private Inventory_Player inventory;

    [Header("PLAYER Drop Details")]
    [SerializeField, Range(0, 100)] private float chanceToLoseItem = 10f;

    private void Awake()
    {
        inventory = GetComponent<Inventory_Player>();   
    }

    public override void DropItems()
    {
        List<Inventory_Item> inventoryCopy = new List<Inventory_Item>(inventory.itemList);
        List<Inventory_EquipmentSlot> equipCopy = new List<Inventory_EquipmentSlot>(inventory.equipList);

        foreach (var item in inventoryCopy)
        {
            if(Random.Range(0, 100) < chanceToLoseItem)
            {
                CreateItemDrop(item.itemData);
                inventory.RemoveAllItems(item);
            }
        }

        foreach(var equip in equipCopy)
        {
            if (Random.Range(0, 100) < chanceToLoseItem && equip.HasItem())
            {
                var item = equip.equippedItem;

                CreateItemDrop(item.itemData);
                inventory.UnequipItem(item);
                inventory.RemoveAllItems(item);
            }
        }
    }
}
