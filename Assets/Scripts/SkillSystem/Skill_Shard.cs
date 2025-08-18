using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonateTime = 2f;

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 6;

    [Header("MultiCast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isRecharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f;

    [Header("HBealth Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        if (Unlocked(Skill_UpgradeType.Shard))
            HandleShardRegular();

        if (Unlocked(Skill_UpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if (Unlocked(Skill_UpgradeType.Shard_MultiCast))
            HandleShardMulticast();

        if (Unlocked(Skill_UpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            HandleShardHealthRewind();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCooldown();
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);

        SetSkillOnCooldown();
    }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0)
            return;

        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        currentCharges--;

        if (isRecharging == false)
            StartCoroutine(ShardRechargeCo());
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
            CreateShard();
        else
        {
            SwapPlayerAndShard();
            SetSkillOnCooldown();
        }
    }

    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthPercent();
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercent);
            SetSkillOnCooldown();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        currentShard.Explode();

        player.TeleportPlayer(shardPosition);
    }

    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;

        while(currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }

        isRecharging = false;
    }


    public void CreateShard()
    {
       float detonationTime = GetDetonateTime();

       GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this);

        if (Unlocked(Skill_UpgradeType.Shard_Teleport) || Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            currentShard.OnExplode += ForceCooldown;
    }

    public float GetDetonateTime()
    {
        if (Unlocked(Skill_UpgradeType.Shard_Teleport) || Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            return shardExistDuration;

        return detonateTime;
    }

    private void ForceCooldown()
    {
        if (OnCooldown() == false)
        { 
            SetSkillOnCooldown();
            currentShard.OnExplode -= ForceCooldown;
        }
    }
}
