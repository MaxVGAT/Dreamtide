using TMPro;
using UnityEngine;

public class UI_StatTooltip : UI_Tooltip
{
    private Player_Stats playerStats;
    private TextMeshProUGUI statToolTipText;

    protected override void Awake()
    {
        base.Awake();

        playerStats = FindFirstObjectByType<Player_Stats>();
        statToolTipText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void ShowToolTip(bool show, RectTransform targetRect, StatType statType)
    {
        base.ShowToolTip(show, targetRect);
        statToolTipText.text = GetStatTextByType(statType);
    }

    public string GetStatTextByType(StatType type)
    {
        switch (type)
        {
            // --- Major stats ---
            case StatType.Strength:
                return "1ポイントごとに物理ダメージ" + ColorText("+1", "#00FF00") +
                       "\n1ポイントごとにクリティカル威力" + ColorText("+0.5%", "#00FF00");

            case StatType.Agility:
                return "1ポイントごとにクリティカル率" + ColorText("+0.3%", "#00FF00") +
                       "\n1ポイントごとに回避率" + ColorText("+0.5%", "#00FF00");

            case StatType.Intelligence:
                return "1ポイントごとに属性耐性" + ColorText("+0.5%", "#00FF00") +
                       "\n1ポイントごとに属性ダメージ" + ColorText("+1", "#00FF00") +
                       "\n属性ダメージがある場合のみボーナス適用";

            case StatType.Vitality:
                return "1ポイントごとに最大HP" + ColorText("+5", "#00FF00") +
                       "\n1ポイントごとにアーマー" + ColorText("+1", "#00FF00");


            // --- Defense Stats ---
            case StatType.MaxHealth:
                return "最大HPを決定する";

            case StatType.HealthRegen:
                return "毎秒回復するHP量";

            case StatType.Armor:
                return "受ける物理ダメージを軽減" +
                       "\n" + ColorText("上限85%", "#FF0000") +
                       "\n現在の軽減率: " + ColorText((playerStats.GetArmorMitigation(0) * 100).ToString("F1") + "%", "#00FF00");

            case StatType.Evasion:
                return "攻撃を完全に回避する確率" +
                       "\n" + ColorText("上限85%", "#FF0000");


            // --- Elemental Resistance Stats ---
            case StatType.FireResistance:
                return "受ける火属性ダメージを軽減";
            case StatType.IceResistance:
                return "受ける氷属性ダメージを軽減";
            case StatType.LightningResistance:
                return "受ける雷属性ダメージを軽減";


            // --- Physical Damage Stats ---
            case StatType.Damage:
                return "物理攻撃のダメージ量を決定する";

            case StatType.CritChance:
                return "攻撃がクリティカルになる確率を決定する";

            case StatType.CritPower:
                return "クリティカル時のダメージ倍率を決定する";

            case StatType.ArmorReduction:
                return "攻撃が敵のアーマーを無視する割合を決定する";

            case StatType.AttackSpeed:
                return "攻撃の速度を決定する";


            // --- Elemental Damage Stats ---
            case StatType.FireDamage:
                return "与える火属性ダメージ量を決定する";
            case StatType.IceDamage:
                return "与える氷属性ダメージ量を決定する";
            case StatType.LightningDamage:
                return "与える雷属性ダメージ量を決定する";

            case StatType.ElementalDamage:
                return "全ての属性ダメージを合算" +
                       "\n最も高い属性が" + ColorText("100%", "#00FF00") + "適用され、状態異常を付与" +
                       "\n他の2属性は" + ColorText("50%", "#00FF00") + "のダメージをボーナスとして加算";

            default:
                return "このステータスには説明が設定されていない";
        }
    }

    private string ColorText(string text, string hex)
    {
        return $"<color={hex}>{text}</color>";
    }
}
