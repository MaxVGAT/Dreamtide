using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Buff Effect", fileName = "Item Effect data - Buff")]
public class ItemEffect_Buff : Item_EffectDataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply; // 適用するバフ一覧
    [SerializeField] private float duration;                // バフ持続時間
    [SerializeField] private string source = Guid.NewGuid().ToString(); // バフの識別子（重複防止用）

    private Player_Stats playerStats; // プレイヤーのステータス参照（未使用）

    public override bool CanBeUsed(Entity_Player player)
    {
        // 同じsourceのバフが未適用なら使用可能
        if (player.stats.CanApplyBuffOf(source))
        {
            this.player = player; // 基底クラスのplayerに保持
            return true;
        }
        else
        {
            Debug.Log("同じバフは2回適用できない");
            return false;
        }
    }

    public override void ExecuteEffect(Entity_Player player)
    {
        // バフを付与して参照を解放
        player.stats.ApplyBuff(buffsToApply, duration, source);
        player = null;
    }
}
