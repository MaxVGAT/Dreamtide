using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Material item", fileName = "Material data - ")]
public class Item_DataSO : ScriptableObject
{

    [Header("Merchant Details")]
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("Craft Details")]
    public Inventory_Item[] craftRecipe;

    [Header("Drop Details")]
    [Range(0, 1000)] public int itemRarityScale = 100;
    [Range(0, 100)] public float dropChance;
    [Range(0, 100)] public float maxDropChance = 65f;

    [Header("Item Details")]

    public string itemName;
    public Sprite itemIcon;
    public Item_Rarity itemRarity;
    public Item_Type itemType;
    public int maxStackSize = 1;

    [Header("Item effect")]
    public Item_EffectDataSO itemEffect;

    private void OnValidate()
    {
        dropChance = GetDropChance();
    }

    public float GetDropChance()
    {
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarityScale + 1) / maxRarity * 100;

        return Mathf.Min(chance, maxDropChance);
    }
}
