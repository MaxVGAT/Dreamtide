using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// インベントリ内のアイテムデータ管理クラス
[Serializable]
public class Inventory_Item
{
    private string itemID; // アイテムのユニークID（スタック管理用）

    public Item_DataSO itemData; // 元データScriptableObject
    public int stackSize = 1;    // 現在のスタック数

    public ItemModifier[] modifiers { get; private set; } // 装備効果の修正子
    public Item_EffectDataSO itemEffect;

    // Price is now stored and consistent per item
    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }

    // コンストラクタ：ScriptableObjectから生成
    public Inventory_Item(Item_DataSO itemData)
    {
        if (itemData == null)
            throw new ArgumentNullException(nameof(itemData));

        this.itemData = itemData;
        itemEffect = itemData.itemEffect;
        itemID = itemData.itemName + " - " + Guid.NewGuid();

        // Generate stats automatically using item's unique ID as seed
        modifiers = GenerateStats(itemData, itemID.GetHashCode());

        // Generate prices once and store them
        GeneratePrices();
    }

    private void GeneratePrices()
    {
        // Use a separate seed for price generation to avoid affecting stats
        var priceRandom = new System.Random((itemID + "_price").GetHashCode());

        // Default base prices by rarity
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

        // Multiplier per rarity
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

        // Small randomization ±25% using our seeded random
        float randomFactor = (float)(priceRandom.NextDouble() * 0.5 + 0.75); // 0.75 to 1.25

        buyPrice = Mathf.RoundToInt(defaultBasePrice * multiplier * randomFactor);
        sellPrice = buyPrice * 0.35f; // Sell for 35% of buy price
    }

    private ItemModifier[] GenerateStats(Item_DataSO itemData, int seed)
    {
        // Use seeded random for consistent generation
        var random = new System.Random(seed);

        // Decide how many stats based on rarity
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

        // Choose allowed stats based on type
        StatType[] possibleStats = itemData.itemType switch
        {
            Item_Type.Weapon => new StatType[]
                { StatType.Damage, StatType.AttackSpeed, StatType.Strength, StatType.Agility, StatType.Intelligence },

            Item_Type.Helmet or Item_Type.Chest or Item_Type.Pants or Item_Type.Bracers or Item_Type.Boots => new StatType[]
                { StatType.MaxHealth, StatType.Armor, StatType.Evasion, StatType.Strength, StatType.Agility, StatType.Intelligence, StatType.Vitality, },

            Item_Type.Ring => new StatType[]
                { StatType.Evasion, StatType.ArmorReduction, StatType.HealthRegen, StatType.CritChance, StatType.CritPower},

            Item_Type.Rune => new StatType[]
                { StatType.FireResistance, StatType.IceResistance, StatType.LightningResistance, StatType.FireDamage, StatType.IceDamage, StatType.LightningDamage, StatType.ElementalDamage},

            Item_Type.Consumables => new StatType[]
                { StatType.HealthRegen},

            _ => Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray()
        };

        // FIXED: Prevent infinite loop by limiting numberOfStats to available unique stats
        numberOfStats = Math.Min(numberOfStats, possibleStats.Length);

        var selectedStats = new List<ItemModifier>();
        var availableStats = possibleStats.ToList(); // Copy to modify

        for (int i = 0; i < numberOfStats && availableStats.Count > 0; i++)
        {
            int randomIndex = random.Next(0, availableStats.Count);
            var stat = availableStats[randomIndex];
            availableStats.RemoveAt(randomIndex); // Remove to prevent duplicates

            int value = GenerateStatValue(stat, itemData.itemRarity, random);
            selectedStats.Add(new ItemModifier { statType = stat, value = value });
        }

        return selectedStats.ToArray();
    }

    private int GenerateStatValue(StatType stat, Item_Rarity rarity, System.Random random)
    {
        // Base value per stat type
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

        // Rarity multiplier
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

        // Small randomization ±10% using seeded random
        float randomFactor = (float)(random.NextDouble() * 0.2 + 0.9); // 0.9 to 1.1

        return Mathf.RoundToInt(baseValue * multiplier * randomFactor);
    }

    // プレイヤーに修正子を適用
    public void AddModifiers(Entity_Stats playerStats)
    {
        if (playerStats == null) return;

        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify?.AddModifier(mod.value, itemID);
        }
    }

    // プレイヤーから修正子を削除
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        if (playerStats == null) return;

        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify?.RemoveModifier(itemID);
        }
    }

    public void AddItemEffect(Entity_Player player) => itemEffect?.Subscribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    // Equipment_DataSOへのキャスト（装備アイテムの場合）
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }

    // スタック可能判定
    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // スタックを増やす - FIXED: Added bounds checking
    public void AddStack()
    {
        if (stackSize < itemData.maxStackSize)
            stackSize++;
    }

    // スタックを減らす - FIXED: Added bounds checking
    public void RemoveStack()
    {
        if (stackSize > 0)
            stackSize--;
    }

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
            string modType = GetStatTypeText(mod.statType); // ステータス名
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
        else
            sb.AppendLine("");



        return sb.ToString();
    }

    // FIXED: Now returns consistent prices
    public int GetPrice(bool forBuying = false)
    {
        return forBuying ? buyPrice : Mathf.FloorToInt(sellPrice);
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
            case StatType.ElementalDamage: return "元素ダメージ"; // FIXED: Added missing translation
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
}