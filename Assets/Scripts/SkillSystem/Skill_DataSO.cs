using UnityEngine;
using System;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]

public class Skill_DataSO : ScriptableObject
{

    [Header("Skill description")]
    public string skillName;
    [TextArea]
    public string skillDescription;
    public Sprite skillIcon;

    [Header("Unlock & Upgrade")]
    public int cost;
    public bool unlockedByDefault;
    public Skill_Type skillType;
    public UpgradeData upgradeData;
}

[Serializable]
public class UpgradeData
{
    public Skill_UpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
