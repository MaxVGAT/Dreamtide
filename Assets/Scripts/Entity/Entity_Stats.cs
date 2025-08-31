using UnityEngine;




public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO setupSO;

    public Stats_ResourceGroup resources;
    public Stats_OffenseGroup offense;
    public Stats_DefenseGroup defense;
    public Stats_MajorGroup major;

    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }

    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower();

        isCrit = Random.Range(0, 100) < critChance;
        float finalPhysicalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalPhysicalDamage * scaleFactor;
    }

    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue(); // +1 bonus damage per STR
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * 0.3f); // +0.3% crit chance per AGI
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * 1); // +1% crit power per STR

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // +1 per INT

        float highestDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }

        if (lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            element = ElementType.Lightning;
        }

        if (highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f; // Deal 50% bonus damage if not highest damage
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestDamage + +weakerElementsDamage + bonusElementalDamage;

        return finalElementalDamage * scaleFactor;
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f; // Gives 0.5% of elemental resistance per INT

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireResistance.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceResistance.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningResistance.GetValue();
                break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f;
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100;

        return finalResistance;

    }

    public float GetMaxHealth()
    {
        float baseHealth = resources.maxHealth.GetValue();
        float bonusHealth = major.vitality.GetValue() * 5;

        float finalMaxHealth = baseHealth + bonusHealth;
        return finalMaxHealth;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = 70f; // Max mitigation capped at 70%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);
        return finalMitigation;
    }

    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue(); // +1 point per VIT

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

    public Stats GetStatByType(StatType type)
    {
        switch (type)
        {
            //Health
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            //Major stats
            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            //Offense stats
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;

            //Defense stats
            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;

            case StatType.FireResistance: return defense.fireResistance;
            case StatType.IceResistance: return defense.iceResistance;
            case StatType.LightningResistance: return defense.lightningResistance;

            default:
                Debug.LogWarning($"StatType {type} is not implemented yet.");
                return null;
        }
    }

    public void ApplyDefaultStatSetup()
    {
        if(setupSO == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(setupSO.maxHealth);
        resources.healthRegen.SetBaseValue(setupSO.healthRegen);

        major.strength.SetBaseValue(setupSO.strength);
        major.agility.SetBaseValue(setupSO.agility);
        major.intelligence.SetBaseValue(setupSO.intelligence);
        major.vitality.SetBaseValue(setupSO.vitality);

        offense.attackSpeed.SetBaseValue(setupSO.attackSpeed);
        offense.damage.SetBaseValue(setupSO.damage);
        offense.critChance.SetBaseValue(setupSO.critChance);
        offense.critPower.SetBaseValue(setupSO.critPower);
        offense.armorReduction.SetBaseValue(setupSO.armorReduction);

        offense.fireDamage.SetBaseValue(setupSO.fireDamage);
        offense.iceDamage.SetBaseValue(setupSO.iceDamage);
        offense.lightningDamage.SetBaseValue(setupSO.lightningDamage);

        defense.armor.SetBaseValue(setupSO.armor);
        defense.evasion.SetBaseValue(setupSO.evasion);

        defense.iceResistance.SetBaseValue(setupSO.iceResistance);
        defense.fireResistance.SetBaseValue(setupSO.fireResistance);
        defense.lightningResistance.SetBaseValue(setupSO.lightningResistance);
    }
}
