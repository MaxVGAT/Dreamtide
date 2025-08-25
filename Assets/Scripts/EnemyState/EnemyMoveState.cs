using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    public EnemyMoveState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Flip the enemy when close to a hole or next to a wall, for patrolling-like action
        if (enemy.isGrounded == false || enemy.isWallDetected)
            enemy.FlipMethod();
    }

    public override void Update()
    {
        base.Update();

        // Apply direction based on movespeed and diretion
        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDirection, rb.linearVelocityY);

        // Make the enemy idle when next to a hole or next to a wall
        if (enemy.isGrounded == false || enemy.isWallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }
}
