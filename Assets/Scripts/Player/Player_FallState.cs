using UnityEngine;

public class Player_FallState : PlayerAiredState
{
    public Player_FallState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded)
            stateMachine.ChangeState(player.idleState);

        if (player.isWallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
