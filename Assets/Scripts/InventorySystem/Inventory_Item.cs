using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// インベントリ内のアイテムデータ管理クラス
[Serializable]
public class Inventory_Item
{
    private string itemID; // アイテム固有ID（スタック・修正子用）

    public Item_DataSO itemData; // 元データScriptableObject
    public int stackSize = 1;    // スタック数

    public ItemModifier[] modifiers { get; private set; } // 装備効果修正子
    public Item_EffectDataSO itemEffect;                  // アイテム効果

    public int buyPrice { get; private set; }             // 購入価格
    public float sellPrice { get; private set; }          // 売却価格

    // ScriptableObjectから生成
    public Inventory_Item(Item_DataSO itemData)
    {
        if (itemData == null)
            throw new ArgumentNullException(nameof(itemData));

        this.itemData = itemData;
        itemEffect = itemData.itemEffect;
        itemID = itemData.itemName + " - " + Guid.NewGuid();

        // 固有IDで修正子を生成
        modifiers = GenerateStats(itemData, itemID.GetHashCode());

        // 価格を生成して保存
        GeneratePrices();
    }

    // アイテム価格生成
    private void GeneratePrices()
    {
        var priceRandom = new System.Random((itemID + "_price").GetHashCode());

        int defaultBasePrice = itemData.itemRarity switch
        {
            Item_Rarity.Common => 10,
            Item_Rarity.Uncommon => 50,
            Item_Rarity.Rare => 150,
            Item_Rarity.Epic => 400,
            Item_Rarity.Legendary => 800,
            Item_Rarity.Unique => 1500,
            _ => 100
        };

        float multiplier = itemData.itemRarity switch
        {
            Item_Rarity.Common => 1f,
            Item_Rarity.Uncommon => 1.3f,
            Item_Rarity.Rare => 1.8f,
            Item_Rarity.Epic => 2.5f,
            Item_Rarity.Legendary => 4f,
            Item_Rarity.Unique => 6f,
            _ => 1f
        };

        float randomFactor = (float)(priceRandom.NextDouble() * 0.5 + 0.75); // ±25%

        buyPrice = Mathf.RoundToInt(defaultBasePrice * multiplier * randomFactor);
        sellPrice = buyPrice * 0.35f; // 売却価格は35%
    }

    // アイテム修正子生成
    private ItemModifier[] GenerateStats(Item_DataSO itemData, int seed)
    {
        var random = new System.Random(seed);

        int numberOfStats = itemData.itemRarity switch
        {
            Item_Rarity.Common => 1,
            Item_Rarity.Uncommon => 1,
            Item_Rarity.Rare => 2,
            Item_Rarity.Epic => 3,
            Item_Rarity.Legendary => 4,
            Item_Rarity.Unique => 5,
            _ => 1
        };

        StatType[] possibleStats = itemData.itemType switch
        {
            Item_Type.Weapon => new StatType[] { StatType.Damage, StatType.AttackSpeed, StatType.Strength, StatType.Agility, StatType.Intelligence },
            Item_Type.Helmet or Item_Type.Chest or Item_Type.Pants or Item_Type.Bracers or Item_Type.Boots
                => new StatType[] { StatType.MaxHealth, StatType.Armor, StatType.Evasion, StatType.Strength, StatType.Agility, StatType.Intelligence, StatType.Vitality },
            Item_Type.Ring => new StatType[] { StatType.Evasion, StatType.ArmorReduction, StatType.HealthRegen, StatType.CritChance, StatType.CritPower },
            Item_Type.Rune => new StatType[] { StatType.FireResistance, StatType.IceResistance, StatType.LightningResistance, StatType.FireDamage, StatType.IceDamage, StatType.LightningDamage, StatType.ElementalDamage },
            Item_Type.Consumables => new StatType[] { StatType.HealthRegen },
            _ => Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray()
        };

        numberOfStats = Math.Min(numberOfStats, possibleStats.Length);

        var selectedStats = new List<ItemModifier>();
        var availableStats = possibleStats.ToList();

        for (int i = 0; i < numberOfStats && availableStats.Count > 0; i++)
        {
            int randomIndex = random.Next(0, availableStats.Count);
            var stat = availableStats[randomIndex];
            availableStats.RemoveAt(randomIndex);

            int value = GenerateStatValue(stat, itemData.itemRarity, random);
            selectedStats.Add(new ItemModifier { statType = stat, value = value });
        }

        return selectedStats.ToArray();
    }

    // ステータス値生成
    private int GenerateStatValue(StatType stat, Item_Rarity rarity, System.Random random)
    {
        int baseValue = stat switch
        {
            StatType.MaxHealth => 10,
            StatType.HealthRegen => 2,
            StatType.Strength => 3,
            StatType.Agility => 3,
            StatType.Intelligence => 3,
            StatType.Vitality => 5,
            StatType.AttackSpeed => 1,
            StatType.Damage => 5,
            StatType.CritChance => 2,
            StatType.CritPower => 5,
            StatType.ArmorReduction => 2,
            StatType.FireDamage => 4,
            StatType.IceDamage => 4,
            StatType.LightningDamage => 4,
            StatType.Armor => 5,
            StatType.Evasion => 2,
            StatType.IceResistance => 3,
            StatType.FireResistance => 3,
            StatType.LightningResistance => 3,
            StatType.ElementalDamage => 2,
            _ => 1
        };

        float multiplier = rarity switch
        {
            Item_Rarity.Common => 1f,
            Item_Rarity.Uncommon => 1.3f,
            Item_Rarity.Rare => 1.8f,
            Item_Rarity.Epic => 2.5f,
            Item_Rarity.Legendary => 4f,
            Item_Rarity.Unique => 6f,
            _ => 1f
        };

        float randomFactor = (float)(random.NextDouble() * 0.2 + 0.9); // ±10%

        return Mathf.RoundToInt(baseValue * multiplier * randomFactor);
    }

    // 修正子適用
    public void AddModifiers(Entity_Stats playerStats)
    {
        if (playerStats == null) return;

        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify?.AddModifier(mod.value, itemID);
        }
    }

    // 修正子削除
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        if (playerStats == null) return;

        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify?.RemoveModifier(itemID);
        }
    }

    // アイテム効果適用/解除
    public void AddItemEffect(Entity_Player player) => itemEffect?.Subscribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    // 装備データ取得
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;
        return null;
    }

    // スタック可能判定
    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // スタック増減
    public void AddStack() { if (stackSize < itemData.maxStackSize) stackSize++; }
    public void RemoveStack() { if (stackSize > 0) stackSize--; }

    // アイテム情報文字列取得
    public string GetItemInfo(bool showForShop = false)
    {
        StringBuilder sb = new StringBuilder();

        if (itemData.itemType == Item_Type.Material)
        {
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("クラフティング専用なリソース。");
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }

        if (itemData.itemType == Item_Type.Consumables)
        {
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine(itemEffect?.effectDescription ?? "効果なし");
            sb.AppendLine("");
            return sb.ToString();
        }

        sb.AppendLine("");
        sb.AppendLine("");

        foreach (var mod in modifiers)
        {
            string modType = GetStatTypeText(mod.statType);
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine(modType + " - " + " +" + modValue);
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("<color=#98FF98>" + itemEffect.effectDescription + "</color>");
            sb.AppendLine("");
            sb.AppendLine("");
        }
        else sb.AppendLine("");

        return sb.ToString();
    }

    // 価格取得
    public int GetPrice(bool forBuying = false) => forBuying ? buyPrice : Mathf.FloorToInt(sellPrice);

    // ステータス名日本語変換
    private string GetStatTypeText(StatType type)
    {
        return type switch
        {
            StatType.MaxHealth => "体力",
            StatType.HealthRegen => "回復力",
            StatType.Strength => "破壊力",
            StatType.Agility => "俊敏さ",
            StatType.Intelligence => "魔力",
            StatType.Vitality => "耐久力",
            StatType.AttackSpeed => "連撃速度",
            StatType.Damage => "攻撃力",
            StatType.CritChance => "会心率",
            StatType.CritPower => "会心ダメージ",
            StatType.ArmorReduction => "防御貫通",
            StatType.FireDamage => "火炎ダメージ",
            StatType.IceDamage => "氷結ダメージ",
            StatType.LightningDamage => "雷撃ダメージ",
            StatType.Armor => "防御力",
            StatType.Evasion => "回避率",
            StatType.IceResistance => "氷耐性",
            StatType.FireResistance => "火耐性",
            StatType.LightningResistance => "雷耐性",
            StatType.ElementalDamage => "元素ダメージ",
            _ => type.ToString()
        };
    }

    // パーセント表記が必要か
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
}
