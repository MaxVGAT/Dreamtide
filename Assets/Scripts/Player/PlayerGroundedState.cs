using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && player.isGrounded == false)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if (input.Player.Block.WasPressedThisFrame())
            stateMachine.ChangeState(player.blockState);

        if(input.Player.Counter.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttackState);
    }
}
