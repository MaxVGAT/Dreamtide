using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.isGrounded == false || enemy.isWallDetected)
            enemy.FlipMethod();
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection, rb.linearVelocityY);

        if (enemy.isGrounded == false || enemy.isWallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }
}
