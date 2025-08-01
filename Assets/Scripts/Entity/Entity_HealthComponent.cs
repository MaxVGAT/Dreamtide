using UnityEngine;

public class Entity_HealthComponent : MonoBehaviour, IDamageable
{

    private Entity_VFX entityVfx;
    private Entity entity;

    [SerializeField] protected float currentHp;
    [SerializeField] protected float maxHp = 100;
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

        currentHp = maxHp;
    }

    //Applies damage and triggers hit VFX. Ignore if dead.
    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;


        Vector2 knockback = CalculateKnockback(damage, damageDealer);
        float duration = CalculateKnockbackDuration(damage);

        if (entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow);
            damage /= 2;
            
        }
        else
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Red);
            entity?.ReceiveKnockback(knockback, duration);
        }

        ReduceHP(damage);
    }

    // Reduces health and checks for death.
    protected virtual void ReduceHP(float damage)
    {
        currentHp -= damage;

        Debug.Log("Took " + damage + "damages");
        if (currentHp <= 0)
            Die();
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

    private bool IsHeavyDamage(float damage) => damage / maxHp > heavyDamageThreshold;
}
