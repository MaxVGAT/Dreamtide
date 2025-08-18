using UnityEngine;
using System;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]

public class Skill_DataSO : ScriptableObject
{
    public int cost;
    public bool unlockedByDefault;
    public Skill_Type skillType;
    public UpgradeData upgradeData;

    [Header("Skill description")]
    public string skillName;
    [TextArea]
    public string skillDescription;
    public Sprite skillIcon;
}

[Serializable]
public class UpgradeData
{
    public Skill_UpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
