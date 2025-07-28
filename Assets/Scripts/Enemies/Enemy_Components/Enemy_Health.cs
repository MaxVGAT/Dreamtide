using UnityEngine;

public class Enemy_Health : Entity_HealthComponent
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>();
    public override void TakeDamage(float damage, Transform damageDealer)
    {
        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        base.TakeDamage(damage, damageDealer);
    }
}
