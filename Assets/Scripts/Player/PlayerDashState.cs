using UnityEngine;

public class PlayerDashState : PlayerState
{

    private float originalGravityScale;
    private int dashDirection;

    public PlayerDashState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        stateTimer = player.dashDuration;

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();

        player.SetVelocity(player.dashSpeed * dashDirection, 0);

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

        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
    }

    private void CancelDashIfNeeded()
    {
        if(player.isWallDetected)
        {
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
