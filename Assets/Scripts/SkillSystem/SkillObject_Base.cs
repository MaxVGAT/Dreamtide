using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;

    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        foreach(var target in EnemiesAround(t, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            ElementalEffectData effectData = new ElementalEffectData(playerStats, damageScaleData);

            float physDamage = playerStats.GetPhysicalDamage(out bool isCrit, damageScaleData.physical);
            float elemDamage = playerStats.GetElementalDamage(out ElementType element, damageScaleData.elemental);

            damageable.TakeDamage(physDamage, elemDamage, element, transform);

            if (element != ElementType.None)
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element, effectData);
        }
    }

    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach(var enemy in EnemiesAround(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if(distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }

        return target;
    }

    protected Collider2D[] EnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    protected virtual void OnDrawGizmos()
    {
        if(targetCheck == null)
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
