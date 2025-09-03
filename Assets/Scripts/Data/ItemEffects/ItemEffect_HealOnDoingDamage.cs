using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Lifesteal", fileName = "Item Effect data - Life Steal")]
public class ItemEffect_HealOnDoingDamage : Item_EffectDataSO
{
    [SerializeField] private float percentHealedOnAttack = 0.2f;

    public override void Subscribe(Entity_Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage)
    {
        player.health.IncreaseHealth(damage * percentHealedOnAttack);
    }
}
