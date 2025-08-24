using UnityEngine;

public class Skill_Domain : Skill_Base
{
    public bool InstantDomain()
    {
        return upgradeType != Skill_UpgradeType.Domain_Echo
            && upgradeType != Skill_UpgradeType.Domain_Shard;
    }

    public void CreateDomain()
    {
        Debug.Log("Create skill object");
    }
}
