using UnityEngine;

// プレイヤーのダッシュ状態
public class Player_DashState : PlayerState
{
    private float originalGravityScale; // ダッシュ前の重力値
    private int dashDirection;           // ダッシュ方向 (-1:左, 1:右)

    public Player_DashState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ダッシュ開始時のエフェクト
        skillManager.dash.OnStartEffect();
        player.vfx.DoImageEchoEffect(player.dashDuration);

        // 入力に応じたダッシュ方向
        dashDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        // ダッシュ時間の設定
        stateTimer = player.dashDuration;

        // 重力無効化
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        // 無敵状態にする
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        CancelDashIfNeeded();

        // ダッシュ中の移動
        player.SetVelocity(player.dashSpeed * dashDirection, 0);

        // ダッシュ終了判定
        if (stateTimer < 0)
        {
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // ダッシュ終了時のエフェクト
        skillManager.dash.OnEndEffect();

        // 移動と重力を元に戻す
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;

        // ダメージを受けられる状態に戻す
        player.health.SetCanTakeDamage(true);
    }

    // 壁に当たったらダッシュをキャンセル
    private void CancelDashIfNeeded()
    {
        if (player.isWallDetected)
        {
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
