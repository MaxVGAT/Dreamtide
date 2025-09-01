using UnityEngine;
using System;

[Serializable]
// リソース系ステータス（HP関連）をまとめたクラス
public class Stats_ResourceGroup
{
    public Stats maxHealth;   // 最大体力
    public Stats healthRegen; // 体力回復量（毎秒回復など）
}
