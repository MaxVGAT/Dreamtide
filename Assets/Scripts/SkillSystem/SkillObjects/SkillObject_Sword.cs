using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;   // シャードの管理スクリプト参照
    protected Transform playerTransform;       // プレイヤーのTransform参照
    [SerializeField] protected float comebackSpeed = 20;  // プレイヤーに戻る速度
    protected bool canComeBack;                // 剣が戻る状態か
    protected float maxAllowedDistance = 25;   // 剣が遠すぎた場合に自動で戻す距離制限

    protected virtual void Update()
    {
        // 剣の向きを移動方向に合わせる
        transform.right = rb.linearVelocity;

        // 戻り処理
        HandleComeback();
    }

    // 剣の初期設定：管理者と飛ばす方向を設定
    public virtual void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;       // 剣を飛ばす
        this.swordManager = swordManager;

        playerTransform = swordManager.transform.root;   // プレイヤーのTransform取得

        // 攻撃計算用情報取得
        playerStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
    }

    // 外部から剣をプレイヤーに戻す指示
    public void GetSwordBackToPlayer() => canComeBack = true;

    // 剣がプレイヤーに戻る処理
    protected virtual void HandleComeback()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 設定距離を超えたら自動で戻す
        if (distance > maxAllowedDistance)
            GetSwordBackToPlayer();

        if (!canComeBack)
            return;

        // プレイヤーに向かって移動
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);

        // プレイヤーに近づいたら破棄
        if (distance < 0.5f)
            Destroy(gameObject);
    }

    // 敵や地形に接触した場合の処理
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);               // 剣を停止させる
        DamageEnemiesInRadius(transform, 1); // 接触範囲内の敵にダメージ
    }

    // 剣を衝突対象に固定させる
    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;               // 物理停止
        transform.parent = collision.transform; // 衝突対象に親設定
    }
}
