using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Lifesteal", fileName = "Item Effect data - Life Steal")]
public class ItemEffect_HealOnDoingDamage : Item_EffectDataSO
{
    [SerializeField] private float percentHealedOnAttack = 0.2f; // 与ダメージに対する回復割合

    public override void Subscribe(Entity_Player player)
    {
        // ダメージ発生時のイベントに登録
        base.Subscribe(player);
        player.combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void Unsubscribe()
    {
        // イベント登録解除
        base.Unsubscribe();
        player.combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage)
    {
        // 与ダメージ × 割合 で体力回復
        player.health.IncreaseHealth(damage * percentHealedOnAttack);
    }
}
