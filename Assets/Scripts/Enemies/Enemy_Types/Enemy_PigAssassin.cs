using UnityEngine;

public class Enemy_PigAssassin : Entity_Enemy, ICounterable // 豚型アサシン敵クラス
{
    public bool CanBeCountered { get => canBeStunned; } // カウンター可能判定（スタン可能ならtrue）

    // Enemy_VFXやStateMachine用ステート初期化
    protected override void Awake()
    {
        base.Awake();

        // 各ステートを生成してStateMachineに設定
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        deadState = new EnemyDeadState(this, stateMachine, "death");
        stunnedState = new EnemyStunnedState(this, stateMachine, "stunned");
    }

    // Entity_Enemy初期化とステート開始
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期ステートはIdle
    }

    // プレイヤーのカウンター攻撃時の処理
    public void HandleCounterAttack()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState); // スタン状態に遷移
    }
}
