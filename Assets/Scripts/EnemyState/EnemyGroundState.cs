using UnityEngine;

public class EnemyGroundState : EnemyState // 地上にいる間のみ遷移可能な状態を扱うクラス
{
    public EnemyGroundState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 敵が地上にいる時のみプレイヤーを検知したらバトル状態へ遷移
        if (enemy.PlayerIsDetected())
            stateMachine.ChangeState(enemy.battleState);
    }
}
