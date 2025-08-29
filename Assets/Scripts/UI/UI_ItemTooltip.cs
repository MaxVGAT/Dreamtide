using System.Text;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemRarity;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow)
    {
        base.ShowToolTip(show, targetRect);

        itemName.text = itemToShow.itemData.itemName;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = GetItemInfo(itemToShow);
        SetRarityText(itemToShow.itemData.itemRarity);
    }

    public string GetItemInfo(Inventory_Item item)
    {
        if (item.itemData.itemType == Item_Type.Material)
            return "Used for crafting.";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("");

        foreach (var mod in item.modifiers)
        {
            string modType = mod.statType.ToString();
            string modValue = mod.value.ToString();
            sb.AppendLine("+ " + mod.value + " " + modType);
        }

        return sb.ToString();
    }

    private void SetRarityText(Item_Rarity rarity)
    {
        var(color, text) = GetRarityColorAndText(rarity);
        itemRarity.text = text;
        itemRarity.color = color;
    }

    private (Color, string) GetRarityColorAndText(Item_Rarity rarity)
    {
        switch (rarity)
        {
            case Item_Rarity.Common: return (Color.gray, "コモン");
            case Item_Rarity.Uncommon: return (Color.white, "アンコモン");
            case Item_Rarity.Rare: return (Color.blue, "レア");
            case Item_Rarity.Epic: return (new Color(0.64f, 0.21f, 0.93f), "エピック");
            case Item_Rarity.Legendary: return (new Color(1f, 0.5f, 0f), "レジェンダリー");
            default: return (Color.white, "不明");
        }
    }
}
