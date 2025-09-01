using UnityEngine;

// プレイヤーの地上待機状態（Idle）
public class Player_IdleState : PlayerGroundedState
{
    public Player_IdleState(Entity_Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 待機時は横方向の速度をゼロに設定
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // プレイヤーが壁に向かって入力している場合は動かない
        if (player.moveInput.x == player.facingDirection && player.isWallDetected)
            return;

        // 横方向入力がある場合は移動状態に遷移
        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
