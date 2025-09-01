using UnityEngine;

public class Player_WallSlideState : PlayerState
{
    public Player_WallSlideState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    // 毎フレーム更新
    public override void Update()
    {
        base.Update();

        HandleWallSlide();

        // ジャンプ入力で壁ジャンプ状態へ
        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        // 壁を離れたら落下状態へ
        if (!player.isWallDetected)
            stateMachine.ChangeState(player.fallState);

        // 地面に着地したらアイドル状態へ
        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);

            // 進行方向と向きが逆なら反転
            if (player.facingDirection != player.moveInput.x)
                player.FlipMethod();
        }
    }

    // 壁スライド中の移動処理
    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
    }
}
