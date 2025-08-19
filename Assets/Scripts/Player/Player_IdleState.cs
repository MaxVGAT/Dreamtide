using UnityEngine;

public class Player_IdleState : PlayerGroundedState
{
    public Player_IdleState(Entity_Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == player.facingDirection && player.isWallDetected)
            return;

        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
