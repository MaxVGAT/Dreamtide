using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("General details")]
    [SerializeField] protected Skill_Type skillType;
    [SerializeField] protected Skill_UpgradeType upgradeType;
    [SerializeField] private float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed = lastTimeUsed - cooldown;
    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
    }

    public bool CanUseSkill()
    {

        if (upgradeType == Skill_UpgradeType.None)
            return false;

        if (OnCooldown())
            return false;

        //is unlocked?
        // has mana?

        return true;
    }

    protected bool Unlocked(Skill_UpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    private bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    public void ResetCoolDownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;
    public void ResetCooldown() => lastTimeUsed = Time.time;
}
