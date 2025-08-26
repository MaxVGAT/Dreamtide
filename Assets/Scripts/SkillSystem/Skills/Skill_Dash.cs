using UnityEngine;

public class Skill_Dash : Skill_Base
{
    public void OnStartEffect()
    {
        if (Unlocked(Skill_UpgradeType.Dash_CloneOnStart) || Unlocked(Skill_UpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        if (Unlocked(Skill_UpgradeType.Dash_ShardOnStart) || Unlocked(Skill_UpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    public void OnEndEffect()
    {
        if (Unlocked(Skill_UpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        if (Unlocked(Skill_UpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    private void CreateShard() // Skill manager creates time shard
    {
        skillManager.shard.CreateRawShard();
    }

    private void CreateClone() // Skill manager creates clone
    {
        skillManager.timeEcho.CreateTimeEcho();
    }
}
