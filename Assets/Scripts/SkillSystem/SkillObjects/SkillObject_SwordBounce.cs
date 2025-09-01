using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed;   // 次のターゲットに跳ねる速度
    private int bounceCount;                       // 残りの跳ね回数

    private Collider2D[] enemyTargets;            // スキル範囲内の敵
    private Transform nextTarget;                  // 次の跳ねる対象
    private List<Transform> selectedBefore = new List<Transform>(); // 以前選ばれたターゲット記録

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        transform.right = rb.linearVelocity; // 剣の向きを移動方向に合わせる
        HandleComeback();                    // プレイヤーへの戻り処理
        HandleBounce();                      // 次の敵への跳ね処理
    }

    // 次のターゲットへの移動・攻撃処理
    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1); // 衝突時にダメージ

            enemyTargets = GetEnemiesAround(transform, 10); // 周囲の敵を更新
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBackToPlayer(); // 跳ね終了でプレイヤーに戻す
            }
        }
    }

    // 次のターゲットを決定
    private void BounceToNextTarget()
    {
        Transform target = GetNextTarget();
        if (target != null)
        {
            nextTarget = target;
            bounceCount--;
        }
        else
            nextTarget = null;
    }

    // 当たり判定に入ったときの処理
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        anim?.SetTrigger("spin"); // 回転アニメーション

        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10); // 範囲内の敵を取得
            rb.simulated = false;                            // 移動停止
        }

        DamageEnemiesInRadius(transform, 1); // 範囲攻撃

        // 跳ねる対象がいない、または跳ね回数終了
        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBackToPlayer();
        else
            nextTarget = GetNextTarget();
    }

    // aliveな敵だけを返す
    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }

    // 過去に選ばれていない敵だけを返す。すべて選ばれた場合はリストをリセット
    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && !selectedBefore.Contains(enemy.transform))
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)
            return validTargets;

        selectedBefore.Clear();
        return aliveTargets;
    }

    // ランダムに次のターゲットを決定してselectedBeforeに記録
    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();

        if (validTarget.Count == 0)
            return null;

        int randomIndex = Random.Range(0, validTarget.Count);
        Transform nextTarget = validTarget[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }
}
