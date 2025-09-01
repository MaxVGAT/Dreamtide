using UnityEngine;
using System;

[Serializable]
// 防御関連のステータスをまとめたクラス
public class Stats_DefenseGroup
{
    // 物理防御系ステータス
    public Stats armor;   // 防御力。物理ダメージを軽減する
    public Stats evasion; // 回避率。攻撃を避ける確率

    // 属性耐性系ステータス
    public Stats fireResistance;     // 火属性耐性
    public Stats iceResistance;      // 氷属性耐性
    public Stats lightningResistance; // 雷属性耐性
}
