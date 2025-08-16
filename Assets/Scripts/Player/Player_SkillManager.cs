using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash {  get; private set; }

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
    }

    public Skill_Base GetSkillByType(Skill_Type type)
    {
        switch(type)
        {
            case Skill_Type.Dash:
                return dash;
            default:
                Debug.Log("Not implemented");
                return null;
        }
    }
}
