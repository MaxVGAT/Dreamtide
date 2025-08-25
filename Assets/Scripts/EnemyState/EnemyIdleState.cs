using UnityEngine;

public class EnemyIdleState : EnemyGroundState
{
    public EnemyIdleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 敵が各アクション間（壁に当たる、バトル状態から抜けるなど）に待機する時間
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        // 待機時間が0未満になったら移動状態に遷移
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
}
