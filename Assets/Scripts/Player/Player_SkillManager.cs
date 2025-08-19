using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash {  get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
    }

    public Skill_Base GetSkillByType(Skill_Type type)
    {
        switch(type)
        {
            case Skill_Type.Dash:
                return dash;
            case Skill_Type.TimeShard:
                return shard;
            default:
                Debug.Log("Not implemented");
                return null;
        }
    }
}
