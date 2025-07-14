using UnityEngine;

public class PlayerIdleState : EntityState
{
    public PlayerIdleState(StateMachine stateMachine, string stateName) : base(stateMachine, stateName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }
}
