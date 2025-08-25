using UnityEngine;

public class EnemyAttackState : EnemyState // 初期パラメータを適用する基本クラス
{
    public EnemyAttackState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 攻撃速度のステータスをアニメーション速度に同期し、攻撃頻度を合わせる
        SyncAttackSpeed();
    }

    public override void Update()
    {
        base.Update();

        // バトル状態に遷移するトリガーをチェック
        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
