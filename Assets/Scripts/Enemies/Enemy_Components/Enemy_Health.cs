using UnityEngine;

public class Enemy_Health : Entity_HealthComponent
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>();

    public override bool TakeDamage(float damage, float elementalDamage, Transform damageDealer)
    {
        bool wasHit = base.TakeDamage(damage, elementalDamage, damageDealer);

        if (isDead)
            return false;

        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;

    }
}
