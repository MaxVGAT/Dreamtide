using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Material item", fileName = "Material data - ")]
public class Item_DataSO : ScriptableObject
{

    [Header("Merchant Details")]
    [Range(0, 10000)] public int itemPrice = 100;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("Craft Details")]
    public Inventory_Item[] craftRecipe;

    [Header("Item Details")]

    public string itemName;
    public Sprite itemIcon;
    public Item_Rarity itemRarity;
    public Item_Type itemType;
    public int maxStackSize = 1;

    [Header("Item effect")]
    public Item_EffectDataSO itemEffect;

    
}
