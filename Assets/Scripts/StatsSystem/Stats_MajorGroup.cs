using UnityEngine;
using System;

[Serializable]
// メインステータス（主要能力値）をまとめたクラス
public class Stats_MajorGroup
{
    public Stats strength;      // 力：物理攻撃力や一部防御に影響
    public Stats agility;       // 敏捷性：攻撃速度や回避率に影響
    public Stats intelligence;  // 知性：魔法攻撃力やスキル効果に影響
    public Stats vitality;      // 体力：最大HPや耐久力に影響
}
