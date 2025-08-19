using UnityEngine;

public class Player_WallSlideState : PlayerState
{

    public Player_WallSlideState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

            if(player.facingDirection != player.moveInput.x)
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
