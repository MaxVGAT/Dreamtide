 using UnityEngine;
using UnityEngine.UI;

public class Entity_HealthComponent : MonoBehaviour, IDamageable
{
    private Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats stats;

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.6f;

    [Header("On Heavy damages")]
    [SerializeField] private float heavyDamageThreshold = 0.3f; // Percentage of health lost to be heavy damage (eg. if 100hp, above 30 damages would apply a heavy knockback)

    protected virtual void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        healthBar = GetComponentInChildren<Slider>();
        stats = GetComponent<Entity_Stats>();

        currentHp = stats.GetMaxHealth();
        UpdateHealthBar();
    }

    //Applies damage and triggers hit VFX. Ignore if dead.
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead)
            return false;

        if (AttackAvoided())
            return false;

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;
        float physicalDamageTaken, elementalDamageTaken;

        ApplyPhysAndElemRes(damage, elementalDamage, element, armorReduction, out physicalDamageTaken, out elementalDamageTaken);

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHP(physicalDamageTaken + elementalDamageTaken);

        return true;
    }

    private void ApplyPhysAndElemRes(float damage, float elementalDamage, ElementType element, float armorReduction, out float physicalDamageTaken, out float elementalDamageTaken)
    {
        float mitigation = stats.GetArmorMitigation(armorReduction);
        physicalDamageTaken = damage * (1 - mitigation);
        float resistance = stats.GetElementalResistance(element);
        elementalDamageTaken = elementalDamage * (1 - resistance);
    }

    private float TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateKnockbackDuration(finalDamage);

        if (entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow);
            finalDamage /= 2;
        }
        else
            entity?.ReceiveKnockback(knockback, duration);

        return finalDamage;
    }

    private bool AttackAvoided() => Random.Range(0, 100) < stats.GetEvasion();

    // Reduces health and checks for death.
    public void ReduceHP(float damage)
    {
        entityVfx.HandleHitColor(Entity_VFX.FlashType.Red);
        currentHp -= damage;
        UpdateHealthBar();

        if (currentHp <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHp / stats.GetMaxHealth();
    }

    // Death logic - Override for custom behavior(animation, drops...)
    private void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction;

        return knockback;
    }

    private float CalculateKnockbackDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage) => damage / stats.GetMaxHealth() > heavyDamageThreshold;
}
