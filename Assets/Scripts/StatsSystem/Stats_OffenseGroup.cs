using UnityEngine;
using System;

[Serializable]
// 攻撃ステータス（オフェンス系能力値）をまとめたクラス
public class Stats_OffenseGroup
{
    public Stats attackSpeed;   // 攻撃速度：攻撃の間隔に影響

    // 物理攻撃関連
    public Stats damage;        // 基本ダメージ
    public Stats critPower;     // クリティカル時のダメージ倍率
    public Stats critChance;    // クリティカル発生率
    public Stats armorReduction; // 敵の防御力減少量

    // 属性攻撃関連
    public Stats fireDamage;    // 火属性ダメージ
    public Stats iceDamage;     // 氷属性ダメージ
    public Stats lightningDamage; // 雷属性ダメージ
}
