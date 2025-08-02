using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stats maxHealth;
    public Stats_MajorStats major;
    public Stats_OffenseGroup offense;
    public Stats_DefenseGroup defense;


    public float GetMaxHealth()
    {
        float baseHp = maxHealth.GetValue();
        float bonusHP = major.vitality.GetValue() * 5;

        return baseHp + bonusHP;
    }
}
