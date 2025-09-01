using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    // 壁ジャンプ開始時の処理
    public override void Enter()
    {
        base.Enter();

        // プレイヤーに壁ジャンプの初速度を設定
        player.SetVelocity(player.wallJumpDir.x * -player.facingDirection, player.wallJumpDir.y);
    }

    // 毎フレーム更新
    public override void Update()
    {
        base.Update();

        // 上昇が終わったら落下状態へ
        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        // 壁に触れていれば壁スライド状態へ
        if (player.isWallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
