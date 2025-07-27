using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyBattleState : EnemyState
{

    private Transform player;
    private float lastTimeInBattle;

    public EnemyBattleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        if (player == null)
        {
            player = enemy.PlayerIsDetected().transform;
        }

        if(ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }
        
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerIsDetected())
            UpdateBattleTimer();

        if (BattleTimeIsOver())
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerIsDetected())
            stateMachine.ChangeState(enemy.attackState);
        else
        {
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocityY);
        }
    }

    private void UpdateBattleTimer() => lastTimeInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeInBattle + enemy.battleTimeDuration;

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;
        
    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
