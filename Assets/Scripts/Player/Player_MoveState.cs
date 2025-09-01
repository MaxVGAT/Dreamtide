using UnityEngine;

// プレイヤーの移動状態（地面にいる間の左右移動管理）
public class Player_MoveState : PlayerGroundedState
{
    public Player_MoveState(Entity_Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 移動入力がなくなった、または壁に接触したらアイドル状態に移行
        if (player.moveInput.x == 0 || player.isWallDetected)
            stateMachine.ChangeState(player.idleState);

        // 横移動速度を設定（y方向速度は維持）
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }
}
