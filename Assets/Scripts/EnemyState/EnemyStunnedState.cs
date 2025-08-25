using UnityEngine;

public class EnemyStunnedState : EnemyState
{

    private Enemy_VFX enemyVfx;

    public EnemyStunnedState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        // 攻撃アラートのゲームオブジェクトがアタッチされている Enemy_VFX コンポーネントを取得
        enemyVfx = enemy.GetComponent<Enemy_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        // 入ったときにすべてのアクションを無効化
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterAttack(false);
        stateTimer = enemy.stunnedDuration;

        // 正しい方向を適用しつつ、移動速度を0に設定
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDirection, enemy.stunnedVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // スタン状態が終わったらバトル状態に戻る
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.battleState);
    }
}
