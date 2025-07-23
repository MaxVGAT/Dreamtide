using UnityEngine;

public class PlayerWallSlideState : PlayerState
{

    public PlayerWallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        

        HandleWallSlide();

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if (player.isWallDetected == false)
            stateMachine.ChangeState(player.fallState);

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            player.FlipMethod();
        }
    }

    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        else
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
    }

}
