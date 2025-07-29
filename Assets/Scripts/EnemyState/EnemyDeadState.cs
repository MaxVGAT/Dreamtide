using System.Collections;
using UnityEngine;

public class EnemyDeadState : EnemyState
{

    private float deathAnimDuration = 2f;

    public EnemyDeadState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.StartCoroutine(WaitAndDespawn());
    }

    private IEnumerator WaitAndDespawn()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        enemy.DespawnOnDeath(2f);
    }
}
