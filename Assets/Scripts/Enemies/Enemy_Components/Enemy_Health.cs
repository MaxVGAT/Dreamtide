using UnityEngine;

public class Enemy_Health : Entity_HealthComponent
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>();

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);

        if (isDead)
            return;

        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

    }
}
