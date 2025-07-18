using UnityEngine;

public class PlayerGroundedState : EntityState
{
    public PlayerGroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        if (inputActions.Player.Jump.WasPerformedThisFrame())
            stateMachine.ChangeState(player.jumpState);
    }
}
