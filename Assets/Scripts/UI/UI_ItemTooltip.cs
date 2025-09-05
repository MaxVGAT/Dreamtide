using System.Text;
using TMPro;
using UnityEngine;

// アイテムのツールチップUIを管理するクラス
public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;   // アイテム名
    [SerializeField] private TextMeshProUGUI itemRarity; // レアリティ
    [SerializeField] private TextMeshProUGUI itemType;   // アイテム種別
    [SerializeField] private TextMeshProUGUI itemInfo;   // ステータス詳細

    // ツールチップの表示・非表示
    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow)
    {
        // if no item, never show tooltip
        if (itemToShow == null)
            show = false;

        base.ShowToolTip(show, targetRect);

        if (!show)
        {
            itemName.text = "";
            itemType.text = "";
            itemInfo.text = "";
            itemRarity.text = "";
            return;
        }

        // rest of your code unchanged
        itemName.text = itemToShow.itemData.itemName;
        itemType.text = SetItemTypeJP(itemToShow.itemData.itemType);
        itemInfo.text = GetItemInfo(itemToShow);
        SetRarityText(itemToShow.itemData.itemRarity);
    }

    // アイテムのステータス情報を文字列で返す
    public string GetItemInfo(Inventory_Item item)
    {
        if (item.itemData.itemType == Item_Type.Material)
            return "クラフティング専用なリソース。";

        if (item.itemData.itemType == Item_Type.Consumables)
            return item.itemData.itemEFfect.effectDescription;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("");

        foreach (var mod in item.modifiers)
        {
            string modType = GetStatTypeText(mod.statType); // ステータス名
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine(modType + " - " + " +" + modValue);
        }

        if(item.itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("Unique Effect: ");
            sb.AppendLine(item.itemEffect.effectDescription);
        }

        return sb.ToString();
    }

    // レアリティ表示を更新
    private void SetRarityText(Item_Rarity rarity)
    {
        var (color, text) = GetRarityColorAndText(rarity);
        itemRarity.text = text;
        itemRarity.color = color;
    }

    // レアリティに応じた色とテキストを取得
    private (Color, string) GetRarityColorAndText(Item_Rarity rarity)
    {
        switch (rarity)
        {
            case Item_Rarity.Common: return (Color.gray, "コモン");
            case Item_Rarity.Uncommon: return (Color.white, "アンコモン");
            case Item_Rarity.Rare: return (Color.blue, "レア");
            case Item_Rarity.Epic: return (new Color(0.64f, 0.21f, 0.93f), "エピック");
            case Item_Rarity.Legendary: return (new Color(1f, 0.5f, 0f), "レジェンダリー");
            case Item_Rarity.Unique: return (new Color(0, 1f, 0.73f), "ユニック");
            default: return (Color.white, "不明");
        }
    }

    // ステータスの日本語表示を返す
    private string GetStatTypeText(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "体力";
            case StatType.HealthRegen: return "回復力";
            case StatType.Strength: return "破壊力";
            case StatType.Agility: return "俊敏さ";
            case StatType.Intelligence: return "魔力";
            case StatType.Vitality: return "耐久力";
            case StatType.AttackSpeed: return "連撃速度";
            case StatType.Damage: return "攻撃力";
            case StatType.CritChance: return "会心率";
            case StatType.CritPower: return "会心ダメージ";
            case StatType.ArmorReduction: return "防御貫通";
            case StatType.FireDamage: return "火炎ダメージ";
            case StatType.IceDamage: return "氷結ダメージ";
            case StatType.LightningDamage: return "雷撃ダメージ";
            case StatType.Armor: return "防御力";
            case StatType.Evasion: return "回避率";
            case StatType.IceResistance: return "氷耐性";
            case StatType.FireResistance: return "火耐性";
            case StatType.LightningResistance: return "雷耐性";
            default: return type.ToString();
        }
    }

    // パーセンテージ表記が必要か判定
    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
            case StatType.Evasion:
                return true;
            default:
                return false;
        }
    }

    // アイテム種別の日本語表記
    private string SetItemTypeJP(Item_Type type)
    {
        switch (type)
        {
            case Item_Type.Helmet: return "ヘルメット";
            case Item_Type.Shoulders: return "ショルダー";
            case Item_Type.Chest: return "チェスト";
            case Item_Type.Pants: return "ズボン";
            case Item_Type.Cape: return "ケープ";
            case Item_Type.Bracers: return "ブレイサー";
            case Item_Type.Gloves: return "グローブ";
            case Item_Type.Boots: return "ブーツ";
            case Item_Type.Weapon: return "ウェポン";
            case Item_Type.Ring: return "リング";
            case Item_Type.Rune: return "ルーン";
            case Item_Type.Material: return "マテリアル";
            case Item_Type.Consumables: return "コンシュマブル";
            default: return null;
        }
    }
}
