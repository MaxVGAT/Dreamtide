using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    [Header("Attack upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = 0.3f;

    [Header("Heal Wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = 0.3f;
    [SerializeField] private float cooldownReducedInSeconds;

    public float GetPercentOfDamageHealed()
    {
        if (ShouldBeWisp() == false)
            return 0;

        return damagePercentHealed;
    }

    public float GetCooldownReduceInSeconds()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_CooldownWisp)
            return 0;

        return cooldownReducedInSeconds;
    }
    
    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    public bool ShouldBeWisp()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_HealWisp
            || upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp
            || upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    public float GetDuplicateChance()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 0;

        return duplicateChance;
    }

    public int GetMaxAttacks()
    {
        if (upgradeType == Skill_UpgradeType.TimeEcho_SingleAttack || upgradeType == Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;

        if (upgradeType == Skill_UpgradeType.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;
    }

    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        Vector3 exactPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        CreateTimeEcho(exactPosition);
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position;

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
