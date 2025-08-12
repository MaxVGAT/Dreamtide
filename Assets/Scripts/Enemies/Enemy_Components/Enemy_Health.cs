using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>();

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        if (isDead)
            return false;

        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;

    }
}
