using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyBattleState : EnemyState
{

    private Transform player;
    private Transform lastTarget;
    private float lastTimeInBattle;

    public EnemyBattleState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer(); // 敵のバトル状態の総継続時間のタイマーを更新する

        if (player == null)
            player = enemy.GetPlayerReference();

        if (ShouldRetreat()) // プレイヤーとの距離を増やすべきかをチェックし、プレイヤーの方向に基づいて反転し、ダッシュ速度やアクティブなスロウデバフを考慮する
        {
            rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }

    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerIsDetected())
        {
            UpdateTargetIfNeeded(); // 現在のターゲットが同じかどうか、またはクローンやターゲットなしに変わったかをチェックする
            UpdateBattleTimer(); // ターゲットが視界内にいる場合、タイマーを維持する
        }

        if (BattleTimeIsOver()) // プレイヤーが一定時間視界から外れたら待機状態に戻る
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.PlayerIsDetected()) // プレイヤーが近くにいて検出されていれば攻撃する
            stateMachine.ChangeState(enemy.attackState);
        else // プレイヤーが射程外のため、バトル状態の移動速度を上げて接近する
        {
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocityY);
        }
    }

    private void UpdateTargetIfNeeded() // プレイヤーやクローン、または何も検出されていない場合に応じてターゲットを更新する
    {
        if (enemy.PlayerIsDetected() == false)
            return;

        Transform newTarget = enemy.PlayerIsDetected().transform;

        if (newTarget != lastTarget) // 毎フレーム現在のターゲットが同じかどうかをチェックし、異なればターゲットを更新する
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

        return player.position.x > enemy.transform.position.x ? 1 : -1; // プレイヤーの位置に基づいて敵の方向を変更する
    }
}
