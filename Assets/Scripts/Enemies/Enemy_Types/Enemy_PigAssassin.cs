using UnityEngine;

public class Enemy_PigAssassin : Entity_Enemy, ICounterable
{

    protected override void Awake()
    {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        deadState = new EnemyDeadState(this, stateMachine, "death");
        stunnedState = new EnemyStunnedState(this, stateMachine, "stunned");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.H))
            HandleCounterAttack();
    }

    public void HandleCounterAttack()
    {
        if (canBeStunned == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
