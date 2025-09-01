using UnityEngine;

// プレイヤーのジャンプ状態（空中にいる間の移動管理）
public class Player_JumpState : PlayerAiredState
{
    public Player_JumpState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ジャンプ開始時の垂直速度を設定（横速度は維持）
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        // 下方向に移動中で、ジャンプ攻撃中でなければ落下状態に移行
        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }
}
