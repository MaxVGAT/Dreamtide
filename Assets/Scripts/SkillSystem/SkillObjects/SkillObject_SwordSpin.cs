using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;           // プレイヤーからの最大距離
    private float attacksPerSecond;    // 秒間攻撃回数
    private float attackTimer;         // 攻撃タイマー

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        maxDistance = swordManager.maxDistance;               // 最大距離設定
        attacksPerSecond = swordManager.attacksPerSecond;     // 攻撃速度設定

        // 最大回転時間後に剣を戻す
        Invoke(nameof(GetSwordBackToPlayer), swordManager.maxSpinDuration);
    }

    protected override void Update()
    {
        transform.right = rb.linearVelocity; // 剣の向きを速度方向に
        HandleAttack();                      // 範囲攻撃処理
        HandleStopping();                    // 最大距離チェックとアニメーション
        HandleComeback();                    // プレイヤーに戻る処理
    }

    // プレイヤーからの距離が最大距離を超えたら剣を停止
    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistance && rb.simulated == true)
        {
            rb.simulated = false;           // 物理挙動停止

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Spin"))
                anim?.SetTrigger("spin");  // 回転アニメーション開始
        }
    }

    // 範囲攻撃を攻撃速度に応じて行う
    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 0.7f); // 衝突範囲攻撃
            attackTimer = 1 / attacksPerSecond;    // 次の攻撃タイマー設定
        }
    }

    // 衝突時の処理、剣を停止させてアニメーション再生
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        anim?.SetTrigger("spin");
        rb.simulated = false;
    }
}
