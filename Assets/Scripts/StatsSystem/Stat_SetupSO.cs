using UnityEngine;

// キャラクターのデフォルトステータスを保存するScriptableObject
[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("リソース")]
    public float maxHealth = 100; // 最大体力
    public float healthRegen;     // 1秒あたりの体力回復量

    [Header("攻撃力 - 物理ダメージ")]
    public float attackSpeed = 1; // 攻撃速度（秒あたりの攻撃回数）
    public float damage = 10;     // 基本物理ダメージ
    public float critChance;      // クリティカル発生確率
    public float critPower = 150; // クリティカル時のダメージ倍率（%）

    [Header("攻撃力 - 属性ダメージ")]
    public float fireDamage;      // 火属性ダメージ
    public float iceDamage;       // 氷属性ダメージ
    public float lightningDamage; // 雷属性ダメージ

    [Header("防御力 - 物理ダメージ")]
    public float armorReduction;  // 物理ダメージ減少率（%）
    public float evasion;         // 回避率
    public float armor;           // 固定防御力（ダメージ減少に寄与）

    [Header("防御力 - 属性ダメージ")]
    public float fireResistance;      // 火属性ダメージ耐性（%）
    public float iceResistance;       // 氷属性ダメージ耐性（%）
    public float lightningResistance; // 雷属性ダメージ耐性（%）

    [Header("主要ステータス")]
    public float strength;     // 通常、物理攻撃力や近接ダメージに影響
    public float agility;      // 攻撃速度、回避率、移動速度に影響
    public float intelligence; // 魔法攻撃力やスキル効果に影響
    public float vitality;     // 最大体力や耐久力に影響
}
