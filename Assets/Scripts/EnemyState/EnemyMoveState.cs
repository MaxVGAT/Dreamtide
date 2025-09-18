using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    public EnemyMoveState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // ŒŠ‚â•Ç‚Ì‹ß‚­‚Å“G‚ğ”½“]‚³‚¹‚Ä„‰ñ‚Á‚Û‚­“®‚©‚·
        if (enemy.isGrounded == false || enemy.isWallDetected)
            enemy.FlipMethod();
    }

    public override void Update()
    {
        base.Update();

        // ˆÚ“®‘¬“x‚Æ•ûŒü‚É‰‚¶‚Ä“G‚ğˆÚ“®
        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDirection, rb.linearVelocityY);

        // ŒŠ‚â•Ç‚ª‚ ‚éê‡‚ÍIdleó‘Ô‚É‘JˆÚ
        if (enemy.isGrounded == false || enemy.isWallDetected)
            stateMachine.ChangeState(enemy.idleState);
    }
}
