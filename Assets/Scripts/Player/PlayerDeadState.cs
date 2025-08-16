using UnityEditor;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        input.Disable();
        rb.simulated = false;
    }
}
