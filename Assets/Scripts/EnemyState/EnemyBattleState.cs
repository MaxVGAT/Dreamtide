using UnityEngine;

public class EnemyBattleState : EnemyState
{
    private Transform player;          // 現在のターゲットプレイヤー
    private Transform lastTarget;      // 前回のターゲット保持
    private float lastTimeInBattle;    // 戦闘状態に入った時間

    public EnemyBattleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer(); // 戦闘時間を更新

        if (player == null)
            player = enemy.GetPlayerReference(); // プレイヤー参照を取得

        if (ShouldRetreat()) // 最小距離を下回ったら後退
        {
            rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer()); // 向きを調整
        }
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerIsDetected())
        {
            UpdateTargetIfNeeded(); // ターゲット更新
            UpdateBattleTimer();    // 戦闘タイマー更新
        }

        if (BattleTimeIsOver()) // 戦闘時間超過でIdleへ
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerIsDetected()) // 攻撃範囲内ならAttackStateへ
            stateMachine.ChangeState(enemy.attackState);
        else // 範囲外なら移動して接近
        {
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocityY);
        }
    }

    private void UpdateTargetIfNeeded()
    {
        if (enemy.PlayerIsDetected() == false)
            return;

        Transform newTarget = enemy.PlayerIsDetected().transform;

        if (newTarget != lastTarget) // ターゲット変更時のみ更新
        {
            lastTarget = newTarget;
            player = newTarget;
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

        // プレイヤーの位置に応じて方向を返す（右=1, 左=-1）
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
