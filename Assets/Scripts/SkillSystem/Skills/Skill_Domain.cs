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

    [Header("Shard Cast Upgrade")]
    [SerializeField] private int shardsToCast = 10;
    [SerializeField] private float shardCastDomainSlow = 1f;
    [SerializeField] private float shardCastDomainDuration = 5f;
    private float spellCastTimer;
    private float spellsPerSecond;

    [Header("Time Echo Cast Upgrade")]
    [SerializeField] private int echoToCast = 8;
    [SerializeField] private float echoCastDomainSlow = 1f;
    [SerializeField] private float echoCastDomainDuration = 5f;
    [SerializeField] private float healthToRestoreWithWispEcho = 0.05f;

    private List<Entity_Enemy> trappedTargets = new List<Entity_Enemy>();
    private Transform currentTarget;

    public void CreateDomain()
    {
        spellsPerSecond = GetSpellsToCast() / GetDomainDuration();
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
        trappedTargets.RemoveAll(target => target == null || target.health.isDead);

        if (trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);
        return trappedTargets[randomIndex].transform;
    }

    public float GetDomainDuration()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownDomainDuration;
        else if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoCastDomainDuration;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardCastDomainDuration;

        return 0;
    }

    public float GetSlowPercentage()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownPercent;
        else if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoCastDomainSlow;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardCastDomainSlow;

        return 0;
    }

    private int GetSpellsToCast()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoToCast;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardsToCast;

        return 0;
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
