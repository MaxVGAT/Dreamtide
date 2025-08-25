using UnityEngine;

public class Enemy_PigAssassin : Entity_Enemy, ICounterable // ピッグアサシン敵固有の詳細を扱うクラス
{
    public bool CanBeCountered { get => canBeStunned; } // CanBeCountered は canBeStunned に設定可能なフォローアップ状態用のフラグ

    // Enemy_VFXをオーバーライドして敵のアニメーション状態を適用
    protected override void Awake()
    {
        base.Awake();

        // 各状態をそれぞれのスクリプトとアニメーションで初期化
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        deadState = new EnemyDeadState(this, stateMachine, "death");
        stunnedState = new EnemyStunnedState(this, stateMachine, "stunned");
    }

    // Entity親スクリプトからアイドル状態を初期化
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    // カウンター攻撃のタイミングでブロックされた場合、stunnedStateに状態を変更
    public void HandleCounterAttack()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
