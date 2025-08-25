using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private int spellsToCast = 10;
    [SerializeField] private float spellCastingDomainSlowDown = 1f;
    [SerializeField] private float spellCastingDomainDuration = 5f;
    private float spellCastTimer;
    private float spellsPerSecond;

    private List<Entity_Enemy> trappedTargets = new List<Entity_Enemy>();
    private Transform currentTarget;

    public void CreateDomain()
    {
        spellsPerSecond = spellsToCast / GetDomainDuration();
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_Domain>().SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;

        if (currentTarget == null)
            currentTarget = FindTargetInDomain();

        if(currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);
            spellCastTimer = 1 / spellsPerSecond;
            currentTarget = null;
        }
    }

    private void CastSpell(Transform target)
    {
        if(upgradeType == Skill_UpgradeType.Domain_Echo)
        {
            Vector3 offset = Random.value < 0.5f ? new Vector2(2, 0) : new Vector2(-2, 0);
            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if(upgradeType == Skill_UpgradeType.Domain_Shard)
        {
            skillManager.shard.CreateRawShard(target, true);
        }
    }



    private Transform FindTargetInDomain()
    {
        if (trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);
        Transform target = trappedTargets[randomIndex].transform;

        if(target == null)
        {
            trappedTargets.RemoveAt(randomIndex);
            return null;
        }

        return target;
    }

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

    public void AddTarget(Entity_Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);
    }

    public void ClearTargets()
    {

        foreach (var enemy in trappedTargets)
            enemy.StopSlowDown();

        trappedTargets = new List<Entity_Enemy>();
    }
}
