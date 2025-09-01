using UnityEngine;

// 属性の種類
public enum ElementType
{
    None,       // 属性なし
    Fire,       // 火属性
    Ice,        // 氷属性
    Lightning   // 雷属性
}

// ステータスの種類
public enum StatType
{
    MaxHealth,      // 最大体力
    HealthRegen,    // 体力回復量
    Strength,       // 力
    Agility,        // 敏捷
    Intelligence,   // 知性
    Vitality,       // 体力・耐久
    AttackSpeed,    // 攻撃速度
    Damage,         // 物理ダメージ
    CritChance,     // クリティカル率
    CritPower,      // クリティカル威力
    ArmorReduction, // 敵防御貫通率
    FireDamage,     // 火属性ダメージ
    IceDamage,      // 氷属性ダメージ
    LightningDamage,// 雷属性ダメージ
    Armor,          // 防御力
    Evasion,        // 回避率
    IceResistance,  // 氷属性耐性
    FireResistance, // 火属性耐性
    LightningResistance, // 雷属性耐性
    ElementalDamage // 総属性ダメージ（計算用）
}
