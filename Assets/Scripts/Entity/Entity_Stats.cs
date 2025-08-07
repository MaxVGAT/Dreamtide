using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stats maxHealth;
    public Stats_MajorStats major;
    public Stats_OffenseGroup offense;
    public Stats_DefenseGroup defense;

    public float GetPhysicalDamage(out bool isCrit)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f; // +0.3% crit chance per AGI
        float critChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * 1; // +1% crit power per STR

        float critPower = (baseCritPower + bonusCritPower) / 100; // Crit power as multiplier (eg. 150 / 100 = 1.5 - multiplier)

        isCrit = Random.Range(0, 100) < critChance;
        float finalPhysicalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;

        return finalPhysicalDamage;
    }

    public float GetElementalDamage()
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // +1 per INT

        float highestDamage = fireDamage;

        if(iceDamage > highestDamage)
            highestDamage = iceDamage;
        else if(lightningDamage > highestDamage)
            highestDamage = lightningDamage;

        if (highestDamage <= 0)
            return 0;

        float bonusFire = (fireDamage == highestDamage) ? 0 : fireDamage * 0.5f; // Deal 50% bonus damage if not highest damage
        float bonusIce = (iceDamage == highestDamage) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (lightningDamage == highestDamage) ? 0 : lightningDamage * 0.5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestDamage + + weakerElementsDamage + bonusElementalDamage;

        return finalElementalDamage;
    }

    public float GetMaxHealth()
    {
        float baseHealth = maxHealth.GetValue();
        float bonusHealth = major.vitality.GetValue() * 5;

        float finalMaxHealth = baseHealth + bonusHealth;
        return finalMaxHealth;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue(); // Bonus from vitality, 1 point per VIT
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = 70f; // Max mitigation capped at 70%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);
        return finalMitigation;
    }

    public float GetArmorReduction()
    {
        float finalArmorReduction = offense.armorReduction.GetValue() / 100;

        return finalArmorReduction;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // each agility point gives +0.5% evasion

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 25f; // Evasion will be capped at 50%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }
}
