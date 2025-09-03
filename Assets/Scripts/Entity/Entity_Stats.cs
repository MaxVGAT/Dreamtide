using UnityEngine;

// �L�����N�^�[�̃X�e�[�^�X��Ǘ�����N���X
public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO setupSO; // �f�t�H���g�X�e�[�^�X�ݒ�

    public Stats_ResourceGroup resources; // �̗͂�񕜂Ȃǂ̃��\�[�X
    public Stats_OffenseGroup offense; // �U���n�X�e�[�^�X
    public Stats_DefenseGroup defense; // �h��n�X�e�[�^�X
    public Stats_MajorGroup major; // ��{�\�͒l

    protected virtual void Awake()
    {

    }

    // �U���f�[�^��擾
    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }


    // �����_���[�W�v�Z
    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage(); // ��{�_���[�W
        float critChance = GetCritChance(); // �N���e�B�J����
        float critPower = GetCritPower(); // �N���e�B�J���{��

        isCrit = Random.Range(0, 100) < critChance; // �N���e�B�J������
        float finalPhysicalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalPhysicalDamage * scaleFactor; // �X�P�[����|���ĕԂ�
    }

    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue(); // STR�ɂ��{�[�i�X
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * 0.3f); // AGI�ɂ��N�������Z
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * 1); // STR�ɂ��N���{�����Z

    // �����_���[�W�v�Z
    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // INT�ɂ��{�[�i�X

        float highestDamage = fireDamage;
        element = ElementType.Fire;

        // ��ԍ��������_���[�W�����
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
            element = ElementType.None; // �����Ȃ�
            return 0;
        }

        // ��������50%�_���[�W��ǉ�
        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestDamage + weakerElementsDamage + bonusElementalDamage;

        float elementalDamage = finalElementalDamage * scaleFactor;

        return elementalDamage;
    }

    // �����ϐ��擾
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f; // INT��0.5%�ǉ�

        switch (element)
        {
            case ElementType.Fire: baseResistance = defense.fireResistance.GetValue(); break;
            case ElementType.Ice: baseResistance = defense.iceResistance.GetValue(); break;
            case ElementType.Lightning: baseResistance = defense.lightningResistance.GetValue(); break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f; // �ő�ϐ�75%
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100;

        return finalResistance;
    }

    // �ő�̗͌v�Z
    public float GetMaxHealth()
    {
        float baseHealth = resources.maxHealth.GetValue();
        float bonusHealth = major.vitality.GetValue() * 5; // VIT�ɂ��{�[�i�X
        float finalMaxHealth = baseHealth + bonusHealth;
        return finalMaxHealth;
    }

    // �A�[�}�[�ɂ��_���[�W�y���v�Z
    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();
        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1); // �A�[�}�[������K�p
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100); // ��{�y���v�Z
        float mitigationCap = 70f; // �y���ő�70%
        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue(); // VIT�ɂ��A�[�}�[���Z

    public float GetArmorReduction()
    {
        float finalArmorReduction = offense.armorReduction.GetValue() / 100; // %�v�Z
        return finalArmorReduction;
    }

    // ��𗦌v�Z
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // AGI�ɂ��{�[�i�X

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 25f; // �ő�25%
        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }

    // �X�e�[�^�X��^�C�v�ʂŎ擾
    public Stats GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            case StatType.ElementalDamage: return offense.elementalDamage;

            case StatType.FireResistance: return defense.fireResistance;
            case StatType.IceResistance: return defense.iceResistance;
            case StatType.LightningResistance: return defense.lightningResistance;

            default:
                Debug.LogWarning($"StatType {type} is not implemented yet.");
                return null;
        }
    }

    // �f�t�H���g�X�e�[�^�X��K�p
    public void ApplyDefaultStatSetup()
    {
        if (setupSO == null)
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
