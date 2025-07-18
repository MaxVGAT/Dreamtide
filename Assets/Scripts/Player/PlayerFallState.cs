using UnityEngine;

public class PlayerFallState : PlayerAiredState
{
    public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            Debug.Log("...");
        }
    }
}
