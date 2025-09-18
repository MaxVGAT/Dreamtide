using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Heal", fileName = "Item Effect data - Heal")]
public class ItemEffect_Heal : Item_EffectDataSO
{
    [SerializeField] private float healPercent = 0.1f; // 最大HPに対する回復割合

    public override void ExecuteEffect(Entity_Player player)
    {
        // 最大HP × 割合 で回復量を計算
        float healAmount = player.stats.GetMaxHealth() * healPercent;

        // プレイヤーのHPを回復
        player.health.IncreaseHealth(healAmount);
    }
}
