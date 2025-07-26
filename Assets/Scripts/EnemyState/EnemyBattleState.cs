using UnityEngine;

public class EnemyBattleState : EnemyState
{
    public EnemyBattleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("ENTERED BATTLE STATE");
    }

}
