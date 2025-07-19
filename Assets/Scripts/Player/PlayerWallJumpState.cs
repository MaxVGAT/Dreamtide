using UnityEngine;

public class PlayerWallJumpState : EntityState
{
    public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(player.wallJumpDir.x * -player.facingDirection, player.wallJumpDir.y);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        if (player.isWallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }


    //POTENTIAL NEW JUMP LOGIC FOR CLIMBING

    //private void HandleJumpDirection()
    //{
    //    if (player.moveInput.x == 1 && player.isWallDetected && player.facingDirection == 1)
    //        player.SetVelocity(player.wallJumpDir.x * player.facingDirection, player.wallJumpDir.y) ;


    //    else if (player.moveInput.x == -1)
    //        Debug.Log("Jump left");

    //    else if (player.moveInput.x == 0)
    //        Debug.Log("Jump up");
    //}

}
