using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]

public class Skill_DataSO : ScriptableObject
{
    public int cost;

    [Header("Skill description")]
    public string skillName;
    [TextArea]
    public string skillDescription;
    public Sprite skillIcon;
}
