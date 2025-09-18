using UnityEngine;
using System;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill description")]
    public string skillName;          // スキル名
    [TextArea]
    public string skillDescription;   // スキル説明
    public Sprite skillIcon;          // スキルアイコン

    [Header("Unlock & Upgrade")]
    public int cost;                  // スキル習得コスト
    public bool unlockedByDefault;    // デフォルトで解放されているか
    public Skill_Type skillType;      // スキルタイプ（アクティブ/パッシブ等）
    public UpgradeData upgradeData;   // スキルのアップグレード情報
}

[Serializable]
public class UpgradeData
{
    public Skill_UpgradeType upgradeType; // アップグレードタイプ
    public float cooldown;                // クールタイム
    public DamageScaleData damageScaleData; // ダメージ倍率データ
}
