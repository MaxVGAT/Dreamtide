using TMPro;
using UnityEngine;

public class UI_StatSlot : MonoBehaviour
{
    private Entity_Stats playerStats;
    private RectTransform rect;
    private UI ui;

    [SerializeField] private StatType statSlotType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;

    private void OnValidate()
    {
        gameObject.name = "UI_Stat - " + GetStatNameByType(statSlotType);
        statName.text = GetStatNameByType(statSlotType);
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        playerStats = FindFirstObjectByType<Entity_Stats>();
    }

    public void UpdateStatValue()
    {
        Stats statToUpdate = playerStats.GetStatByType(statSlotType);

        if (statToUpdate == null)
            return;

        float value = 0;

        switch (statSlotType)
        {
            // Major stats
            case StatType.Strength:
                value = playerStats.major.strength.GetValue();
                break;
            case StatType.Agility:
                value = playerStats.major.agility.GetValue();
                break;
            case StatType.Intelligence:
                value = playerStats.major.intelligence.GetValue();
                break;
            case StatType.Vitality:
                value = playerStats.major.vitality.GetValue();
                break;

            // Offensive stats
            case StatType.Damage:
                value = playerStats.GetBaseDamage();
                break;
            case StatType.CritChance:
                value = playerStats.GetCritChance();
                break;
            case StatType.CritPower:
                value = playerStats.GetCritPower();
                break;
            case StatType.ArmorReduction:
                value = playerStats.GetArmorReduction() * 100;
                break;
            case StatType.AttackSpeed:
                value = playerStats.offense.attackSpeed.GetValue() * 100;
                break;

            // Defensive stats
            case StatType.MaxHealth:
                value = playerStats.GetMaxHealth();
                break;
            case StatType.HealthRegen:
                value = playerStats.resources.healthRegen.GetValue();
                break;
            case StatType.Evasion:
                value = playerStats.GetEvasion();
                break;
            case StatType.Armor:
                value = playerStats.GetBaseArmor();
                break;

            // Elemental damage stats 
            case StatType.FireDamage:
                value = playerStats.offense.fireDamage.GetValue();
                break;
            case StatType.IceDamage:
                value = playerStats.offense.iceDamage.GetValue();
                break;
            case StatType.LightningDamage:
                value = playerStats.offense.lightningDamage.GetValue();
                break;
            case StatType.ElementalDamage:
                value = playerStats.GetElementalDamage(out ElementType element, 1);
                break;

            // Elemental resistance stats
            case StatType.FireResistance:
                value = playerStats.GetElementalResistance(ElementType.Fire) * 100;
                break;
            case StatType.IceResistance:
                value = playerStats.GetElementalResistance(ElementType.Ice) * 100;
                break;
            case StatType.LightningResistance:
                value = playerStats.GetElementalResistance(ElementType.Lightning) * 100;
                break;
        }

        statValue.text = IsPercentageStat(statSlotType) ? value + "%" : value.ToString();
    }

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

    private string GetStatNameByType(StatType type)
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
            case StatType.ElementalDamage: return "属性ダメージ";

            case StatType.Armor: return "防御力";
            case StatType.Evasion: return "回避率";

            case StatType.IceResistance: return "氷耐性";
            case StatType.FireResistance: return "火耐性";
            case StatType.LightningResistance: return "雷耐性";

            default: return type.ToString();
        }
    }
}
