using UnityEngine;

public class Entity_HealthComponent : MonoBehaviour
{

    private Entity_VFX entityVfx;

    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected bool isDead;

    protected virtual void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if(isDead) return;

        entityVfx?.PlayOnDamageVfx();
        ReduceHP(damage);
    }

    protected virtual void ReduceHP(float damage)
    {
        maxHp -= damage;

        if (maxHp <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Entity died!");
    }
}
