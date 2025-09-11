using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemListDataSO dropData;

    [Header("Drop restrictions")]
    [SerializeField] private int maxRarityAmount = 1200;
    [SerializeField] private int maxItemsToDrop = 4;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
            DropItems();
    }

    public virtual void DropItems()
    {
        List<Item_DataSO> itemsToDrop = RollDrops();
        int amountToDrop = Mathf.Min(itemsToDrop.Count, maxItemsToDrop);

        for (int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(itemsToDrop[i]);
        }
    }

    protected void CreateItemDrop(Item_DataSO itemToDrop)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop);
    }

    public List<Item_DataSO> RollDrops()
    {
        List<Item_DataSO> possibleDrops = new List<Item_DataSO>();
        List<Item_DataSO> finalDrops = new List<Item_DataSO>();
        float maxRarityAmount = this.maxRarityAmount;

        // Step 1: roll each item based on rarity and drop chance
        foreach(var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();

            if (Random.Range(0, 100) <= dropChance)
                possibleDrops.Add(item);
        }

        // Step 2: sort by rarity (highest to lowest)
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarityScale).ToList();

        // Step 3: Add titems to final drop list until rarity limit is reached

        foreach(var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarityScale)
            {
                finalDrops.Add(item);
                maxRarityAmount -= item.itemRarityScale;
            }
        }

        return finalDrops;
    }
}
