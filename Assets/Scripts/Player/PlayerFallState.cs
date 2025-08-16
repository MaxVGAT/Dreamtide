using UnityEngine;

public class PlayerFallState : PlayerAiredState
{
    public PlayerFallState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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
