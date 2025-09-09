using System;
using System.Text;
using UnityEngine;

// �C���x���g����̃A�C�e���f�[�^�Ǘ��N���X
[Serializable]
public class Inventory_Item
{
    private string itemID; // �A�C�e���̃��j�[�NID�i�X�^�b�N��C���q�p�j

    public Item_DataSO itemData; // ���f�[�^ScriptableObject
    public int stackSize = 1;    // ���݂̃X�^�b�N��

    public ItemModifier[] modifiers { get; private set; } // �������ʂ̏C���q
    public Item_EffectDataSO itemEffect;

    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }

    // �R���X�g���N�^�FScriptableObject���琶��
    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
        buyPrice = itemData.itemPrice;
        sellPrice = itemData.itemPrice * 0.35f;

        modifiers = EquipmentData()?.modifiers; // �����f�[�^������ΏC���q��擾
        itemEffect = itemData.itemEffect;

        itemID = itemData.itemName + " - " + Guid.NewGuid(); // ���j�[�NID����
    }

    // �v���C���[�ɏC���q��K�p
    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemID);
        }
    }

    // �v���C���[����C���q��폜
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemID);
        }
    }

    public void AddItemEffect(Entity_Player player) => itemEffect?.Subscribe (player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe ();

    // Equipment_DataSO�ւ̃L���X�g�i�����A�C�e���̏ꍇ�j
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }

    // �X�^�b�N�\������
    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // �X�^�b�N�𑝂₷
    public void AddStack() => stackSize++;

    // �X�^�b�N����炷
    public void RemoveStack() => stackSize--;

    public string GetItemInfo()
    {
        if (itemData.itemType == Item_Type.Material)
            return "クラフティング専用なリソース。";

        if (itemData.itemType == Item_Type.Consumables)
            return itemData.itemEffect.effectDescription;

        StringBuilder sb = new StringBuilder();
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
            sb.AppendLine("Unique Effect: ");
            sb.AppendLine(itemEffect.effectDescription);
        }

        return sb.ToString();
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

    
}
