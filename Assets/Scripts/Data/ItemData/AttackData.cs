using UnityEngine;
using System;

[Serializable]
public class AttackData
{
    public float physicalDamage; // 特殊効果のない通常攻撃のダメージ
    public float elementalDamage; // 状態異常などの効果を持つ攻撃のダメージ
    public bool isCrit; // 強化されたクリティカル攻撃かどうか
    public ElementType element; // 各攻撃やスキルに適用される属性

    public ElementalEffectData effectData; // ステータスに基づいて決定される元素効果

    // ステータスとスケールに基づいてダメージを計算するコンストラクタ
    public AttackData(Entity_Stats entityStats, DamageScaleData scaleData)
    {
        physicalDamage = entityStats.GetPhysicalDamage(out isCrit, scaleData.physical);
        elementalDamage = entityStats.GetElementalDamage(out element, scaleData.elemental);

        effectData = new ElementalEffectData(entityStats, scaleData);
    }
}
