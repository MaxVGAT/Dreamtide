using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    [Header("Attack upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicatechance = 0.3f;

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

        CreateTimeEcho();
    }

    public void CreateTimeEcho()
    {
        GameObject timeEcho = Instantiate(timeEchoPrefab, transform.position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
