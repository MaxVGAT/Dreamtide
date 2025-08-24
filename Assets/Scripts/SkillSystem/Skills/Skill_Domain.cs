using UnityEngine;

public class Skill_Domain : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("Domain Details")]
    public float maxDomainSize = 10f;
    public float expandSpeed = 10f;

    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;
    [SerializeField] private float slowDownDomainDuration = 5f;

    [Header("Spell Casting Upgrade")]
    [SerializeField] private float spellCastingDomainSlowDown = 1f;
    [SerializeField] private float spellCastingDomainDuration = 5f;

    public float GetDomainDuration()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownDomainDuration;
        else
            return spellCastingDomainDuration;
    }

    public float GetSlowPercentage()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownPercent;
        else
            return spellCastingDomainSlowDown;
    }

    public bool InstantDomain()
    {
        return upgradeType != Skill_UpgradeType.Domain_Echo
            && upgradeType != Skill_UpgradeType.Domain_Shard;
    }

    public void CreateDomain()
    {
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_Domain>().SetupDomain(this);
    }
}
