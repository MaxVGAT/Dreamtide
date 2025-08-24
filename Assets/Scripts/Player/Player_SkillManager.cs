using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_Domain domain { get; private set; }

    private Skill_Base[] allSkills;

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        domain = GetComponentInChildren<Skill_Domain>();

        allSkills = GetComponentsInChildren<Skill_Base>();
    }

    public void ReduceAllSkillsBooldownBy(float amount)
    {
        foreach(var skills in allSkills)
            skills.ReduceCooldownBy(amount);
    }

    public Skill_Base GetSkillByType(Skill_Type type)
    {
        switch (type)
        {
            case Skill_Type.Dash:
                return dash;

            case Skill_Type.TimeShard:
                return shard;

            case Skill_Type.SwordThrow:
                return swordThrow;

            case Skill_Type.TimeEcho:
                return timeEcho;

            case Skill_Type.Domain:
                return domain;

            default:
                Debug.Log("Not implemented");
                return null;
        }
    }
}
