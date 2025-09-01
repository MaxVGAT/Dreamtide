using UnityEngine;

// プレイヤーの落下状態
public class Player_FallState : PlayerAiredState
{
    public Player_FallState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 地面に着いたらアイドル状態に変更
        if (player.isGrounded)
            stateMachine.ChangeState(player.idleState);

        // 壁に触れたら壁滑り状態に変更
        if (player.isWallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
