using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Material item", fileName = "Material data - ")]
public class Item_DataSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public Item_Rarity itemRarity;
    public Item_Type itemType;
    public int maxStackSize = 1;

    [Header("Item effect")]
    public Item_EffectDataSO itemEFfect;

    [Header("Craft Details")]
    public Inventory_Item[] craftRecipe;
}
