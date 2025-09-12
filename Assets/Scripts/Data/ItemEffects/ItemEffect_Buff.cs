using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Buff Effect", fileName = "Item Effect data - Buff")]
public class ItemEffect_Buff : Item_EffectDataSO
{

    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();

    private Player_Stats playerStats;

    public override bool CanBeUsed(Entity_Player player)
    {
        if (player.stats.CanApplyBuffOf(source))
        {
            this.player = player;
            return true;
        }
        else
        {
            Debug.Log("Same buff can't bne applied twice.");
            return false;
        }
    }

    public override void ExecuteEffect(Entity_Player player)
    {
        player.stats.ApplyBuff(buffsToApply, duration, source);
        player = null;
    }
}
