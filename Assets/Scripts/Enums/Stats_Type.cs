using UnityEngine;

// 属性タイプ定義
public enum ElementType
{
    None,       // 無属性
    Fire,       // 火属性
    Ice,        // 氷属性
    Lightning   // 雷属性
}

// ステータスタイプ定義
public enum StatType
{
    MaxHealth,      // 最大体力
    HealthRegen,    // 体力回復速度
    Strength,       // 力
    Agility,        // 素早さ
    Intelligence,   // 知性
    Vitality,       // 耐久力
    AttackSpeed,    // 攻撃速度
    Damage,         // 攻撃力
    CritChance,     // クリティカル発生率
    CritPower,      // クリティカル威力
    ArmorReduction, // 防御貫通
    FireDamage,     // 火属性攻撃力
    IceDamage,      // 氷属性攻撃力
    LightningDamage,// 雷属性攻撃力
    Armor,          // 防御力
    Evasion,        // 回避率
    IceResistance,  // 氷属性耐性
    FireResistance, // 火属性耐性
    LightningResistance, // 雷属性耐性
    ElementalDamage // 総属性ダメージ
}
