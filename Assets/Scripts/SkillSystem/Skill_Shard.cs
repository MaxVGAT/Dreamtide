using UnityEngine;

public class Skill_Shard : Skill_Base
{
    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonateTime = 2f;

    public void CreateShard()
    {
        if (upgradeType == Skill_UpgradeType.None)
            return;

       GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetupShard(detonateTime);
    }
}
