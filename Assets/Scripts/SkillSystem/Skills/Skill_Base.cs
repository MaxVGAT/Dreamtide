using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Entity_Player player {  get; private set; }
    public Player_SkillManager skillManager { get; private set; }

    public DamageScaleData damageScaleData {  get; private set; }

    [Header("General details")]
    [SerializeField] protected Skill_Type skillType;
    [SerializeField] protected Skill_UpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;


    protected virtual void Awake()
    {
        player = GetComponentInParent<Entity_Player>();
        skillManager = GetComponentInParent<Player_SkillManager>();
        lastTimeUsed = lastTimeUsed - cooldown;
        damageScaleData = new DamageScaleData();
    }

    public virtual void TryUseSkill()
    {

    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;
        ResetCooldown();
    }

    public virtual bool CanUseSkill()
    {

        if (upgradeType == Skill_UpgradeType.None)
            return false;

        if (OnCooldown())
            return false;

        //is unlocked?
        // has mana?

        return true;
    }

    protected bool Unlocked(Skill_UpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;
    public void ResetCooldown() => lastTimeUsed = Time.time - cooldown;
}
