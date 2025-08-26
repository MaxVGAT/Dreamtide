using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;

    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    [Header("Health Regen")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    public float lastDamageTaken { get; private set; }

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.6f;

    [Header("On Heavy damages")]
    [SerializeField] private float heavyDamageThreshold = 0.3f; // Percentage of health lost to be heavy damage (eg. if 100hp, above 30 damages would apply a heavy knockback)

    protected virtual void Awake()
    {
        healthBar = GetComponentInChildren<Slider>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();

        SetupHealth();
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    //Applies damage and triggers hit VFX. Ignore if dead.
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead || canTakeDamage == false)
            return false;

        if (AttackAvoided())
            return false;

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;
        float physicalDamageTaken, elementalDamageTaken;

        ApplyPhysAndElemRes(damage, elementalDamage, element, armorReduction, out physicalDamageTaken, out elementalDamageTaken);

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;

        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private void ApplyPhysAndElemRes(float damage, float elementalDamage, ElementType element, float armorReduction, out float physicalDamageTaken, out float elementalDamageTaken)
    {
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;

        physicalDamageTaken = damage * (1 - mitigation);
        elementalDamageTaken = elementalDamage * (1 - resistance);
    }

    private bool AttackAvoided()
    {
        if (entityStats == null)
            return false;
        else
            return Random.Range(0, 100) < entityStats.GetEvasion();
    }

    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        UpdateHealthBar();
    }

    // Reduces health and checks for death.
    public void ReduceHealth(float damage)
    {
        entityVfx?.HandleHitColor(Entity_VFX.FlashType.Red);
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null || entityStats == null)
            return;

        float maxHealth = entityStats.GetMaxHealth();
        if (maxHealth <= 0)
            return;

        healthBar.value = Mathf.Clamp01(currentHealth / maxHealth);
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        UpdateHealthBar();
    }

    // Death logic - Override for custom behavior(animation, drops...)
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    private float TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateKnockbackDuration(finalDamage);

        if (entity != null && entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow);
            finalDamage /= 2;
        }
        else
            entity?.ReceiveKnockback(knockback, duration);

        return finalDamage;
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction;

        return knockback;
    }

    private float CalculateKnockbackDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;
        else
            return damage / entityStats.GetMaxHealth() > heavyDamageThreshold;
    }
}
