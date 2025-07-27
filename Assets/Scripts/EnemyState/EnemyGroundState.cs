using UnityEngine;

public class EnemyGroundState : EnemyState
{
    public EnemyGroundState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        Debug.Log("PlayerDetection: " + enemy.PlayerIsDetected());

        if (enemy.PlayerIsDetected())
            stateMachine.ChangeState(enemy.battleState);
        
    }
}
