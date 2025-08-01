using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{

    private Player_Combat combat;
    private bool counteredSomething;

    public PlayerCounterAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterRecovery();
        counteredSomething = combat.CounterAttackPerformed();
        anim.SetBool("counterAttackPerformed", counteredSomething);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && counteredSomething == false)
            stateMachine.ChangeState(player.idleState);
    }
}
