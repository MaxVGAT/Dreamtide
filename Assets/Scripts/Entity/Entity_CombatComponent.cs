using UnityEngine;

public class Entity_CombatComponent : MonoBehaviour
{

    public float damage = 10;

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Entity_HealthComponent targetHealth = target.GetComponent<Entity_HealthComponent>();

            targetHealth?.TakeDamage(damage, transform);
            Debug.Log(target.name + ": lost " + damage + "HP!");
        }
    }

    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
