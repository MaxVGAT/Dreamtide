using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Heal", fileName = "Item Effect data - Heal")]
public class ItemEffect_Heal : Item_EffectDataSO
{
    [SerializeField] private float healPercent = 0.1f;
    public override void ExecuteEffect()
    {
        Entity_Player player = FindFirstObjectByType<Entity_Player>();

        float healAmount = player.stats.GetMaxHealth() * healPercent;

        player.health.IncreaseHealth(healAmount);
    }
}
