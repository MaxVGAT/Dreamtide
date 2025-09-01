using UnityEngine;

// プレイヤーのカウンター攻撃状態
public class Player_CounterAttackState : PlayerState
{
    private Entity_VFX vfx;          // VFX管理用
    private Player_Combat combat;     // プレイヤーの戦闘クラス
    private bool counteredSomething;  // カウンターが成功したか

    public Player_CounterAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
        vfx = player.GetComponent<Entity_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        // カウンター後の回復時間を設定
        stateTimer = combat.GetCounterRecovery();

        // カウンター攻撃を実行
        bool isCrit;
        counteredSomething = combat.CounterAttackPerformed(out isCrit);

        anim.SetBool("counterAttackPerformed", counteredSomething);

        // カウンターが成功した場合、VFXを生成
        if (counteredSomething && combat.counteredTargetTransform != null)
        {
            player.stats.GetElementalDamage(out ElementType element);
            vfx.CreateOnHitVFX(combat.counteredTargetTransform, isCrit, element);
        }
    }

    public override void Update()
    {
        base.Update();

        // 攻撃中はプレイヤーを停止
        player.SetVelocity(0, rb.linearVelocity.y);

        // アニメーションのトリガーで状態を終了
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        // カウンターに失敗した場合、タイマーで終了
        if (stateTimer < 0 && counteredSomething == false)
            stateMachine.ChangeState(player.idleState);
    }
}
