using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// ステータスUIスロットを管理するクラス
public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Player_Stats playerStats; // プレイヤーのステータスデータ
    private RectTransform rect;        // UIのRectTransform
    private UI ui;                     // 親UIクラス参照

    [SerializeField] private StatType statSlotType;       // このスロットが担当するステータス
    [SerializeField] private TextMeshProUGUI statName;    // スロット名テキスト
    [SerializeField] private TextMeshProUGUI statValue;   // ステータス値テキスト

    private void OnValidate()
    {
        // スロット名をステータスタイプに応じて自動更新
        gameObject.name = "UI_Stat - " + GetStatNameByType(statSlotType);
        statName.text = GetStatNameByType(statSlotType);
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        playerStats = FindFirstObjectByType<Player_Stats>();
    }

    // マウスがスロットに乗った時にステータスツールチップを表示
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statTooltip.ShowToolTip(true, rect, statSlotType);
    }

    // マウスが離れた時にツールチップを非表示
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.ShowToolTip(false, null);
    }

    // ステータス値を更新
    public void UpdateStatValue()
    {
        Stats statToUpdate = playerStats.GetStatByType(statSlotType);

        if (statToUpdate == null)
            return;

        float value = 0;

        switch (statSlotType)
        {
            // 主要ステータス
            case StatType.Strength: value = playerStats.major.strength.GetValue(); break;
            case StatType.Agility: value = playerStats.major.agility.GetValue(); break;
            case StatType.Intelligence: value = playerStats.major.intelligence.GetValue(); break;
            case StatType.Vitality: value = playerStats.major.vitality.GetValue(); break;

            // 攻撃系ステータス
            case StatType.Damage: value = playerStats.GetBaseDamage(); break;
            case StatType.CritChance: value = playerStats.GetCritChance(); break;
            case StatType.CritPower: value = playerStats.GetCritPower(); break;
            case StatType.ArmorReduction: value = playerStats.GetArmorReduction() * 100; break;
            case StatType.AttackSpeed: value = playerStats.offense.attackSpeed.GetValue() * 100; break;

            // 防御系ステータス
            case StatType.MaxHealth: value = playerStats.GetMaxHealth(); break;
            case StatType.HealthRegen: value = playerStats.resources.healthRegen.GetValue(); break;
            case StatType.Evasion: value = playerStats.GetEvasion(); break;
            case StatType.Armor: value = playerStats.GetBaseArmor(); break;

            // 属性攻撃
            case StatType.FireDamage: value = playerStats.offense.fireDamage.GetValue(); break;
            case StatType.IceDamage: value = playerStats.offense.iceDamage.GetValue(); break;
            case StatType.LightningDamage: value = playerStats.offense.lightningDamage.GetValue(); break;
            case StatType.ElementalDamage: value = playerStats.GetElementalDamage(out ElementType element, 1); break;

            // 属性耐性
            case StatType.FireResistance: value = playerStats.GetElementalResistance(ElementType.Fire) * 100; break;
            case StatType.IceResistance: value = playerStats.GetElementalResistance(ElementType.Ice) * 100; break;
            case StatType.LightningResistance: value = playerStats.GetElementalResistance(ElementType.Lightning) * 100; break;
        }

        statValue.text = IsPercentageStat(statSlotType) ? value + "%" : value.ToString();
    }

    // パーセント表記のステータスか判定
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

    // ステータスタイプから日本語名を取得
    private string GetStatNameByType(StatType type)
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
            StatType.ElementalDamage => "属性ダメージ",
            StatType.Armor => "防御力",
            StatType.Evasion => "回避率",
            StatType.IceResistance => "氷耐性",
            StatType.FireResistance => "火耐性",
            StatType.LightningResistance => "雷耐性",
            _ => type.ToString(),
        };
    }
}
