using UnityEngine;

// エンティティのステータス管理
public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO setupSO; // デフォルトステータス設定SO

    public Stats_ResourceGroup resources; // 体力・回復等
    public Stats_OffenseGroup offense; // 攻撃関連
    public Stats_DefenseGroup defense; // 防御関連
    public Stats_MajorGroup major; // 基本能力値（STR/AGI/INT/VIT）

    protected virtual void Awake()
    {

    }

    // 攻撃データ取得
    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }

    // 物理ダメージ計算
    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage(); // 基本ダメージ
        float critChance = GetCritChance(); // クリティカル率
        float critPower = GetCritPower(); // クリティカル倍率

        isCrit = Random.Range(0, 100) < critChance; // クリティカル判定
        float finalPhysicalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalPhysicalDamage * scaleFactor; // スケール適用
    }

    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue(); // STR依存
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * 0.3f); // AGI依存
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * 1); // STR依存

    // 属性ダメージ計算
    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue(); // INT依存

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
            element = ElementType.None; // ダメージなし
            return 0;
        }

        // 弱属性から50%加算
        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestDamage + weakerElementsDamage + bonusElementalDamage;

        return finalElementalDamage * scaleFactor;
    }

    // 属性耐性取得
    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f; // INT依存

        switch (element)
        {
            case ElementType.Fire: baseResistance = defense.fireResistance.GetValue(); break;
            case ElementType.Ice: baseResistance = defense.iceResistance.GetValue(); break;
            case ElementType.Lightning: baseResistance = defense.lightningResistance.GetValue(); break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f; // 上限75%
        return Mathf.Clamp(resistance, 0, resistanceCap) / 100;
    }

    // 最大体力取得
    public float GetMaxHealth()
    {
        float baseHealth = resources.maxHealth.GetValue();
        float bonusHealth = major.vitality.GetValue() * 5; // VIT依存
        return baseHealth + bonusHealth;
    }

    // 防御によるダメージ軽減計算
    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();
        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = 70f; // 上限70%
        return Mathf.Clamp(mitigation, 0, mitigationCap);
    }

    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue(); // VIT依存

    public float GetArmorReduction() => offense.armorReduction.GetValue() / 100; // %

    // 回避率取得
    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // AGI依存

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 25f; // 上限25%
        return Mathf.Clamp(totalEvasion, 0, evasionCap);
    }

    // StatTypeからStats取得
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
                Debug.LogWarning($"StatType {type} は未実装");
                return null;
        }
    }

    // デフォルトステータス適用
    public void ApplyDefaultStatSetup()
    {
        if (setupSO == null)
        {
            Debug.Log("デフォルトステータス未設定");
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
