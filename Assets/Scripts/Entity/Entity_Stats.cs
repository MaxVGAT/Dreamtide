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

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // each agility point gives +0.5% evasion;

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 25f; // Evasion will be capped at 50%;

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }
}
