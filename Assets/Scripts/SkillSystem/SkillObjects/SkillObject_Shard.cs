using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;

    private Skill_Shard shardManager;
    private Transform target;
    private float speed;

    [SerializeField] private GameObject vfxPrefab;

    private void Update()
    {
        if (target == null)
            return;

        // 目標がある場合、一定速度で移動
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // 指定ターゲット（もしくは最も近い敵）に向かって移動開始
    public void MoveTowardsClosestTarget(float speed, Transform newTarget = null)
    {
        target = newTarget != null ? FindClosestTarget() : target;
        this.speed = speed;
    }

    // シャードの基本セットアップ（自動爆発のみ）
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        // 攻撃計算用の情報を取得
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        // 指定時間後に自動爆発
        float detonationTime = shardManager.GetDetonateTime();
        Invoke(nameof(Explode), detonationTime);
    }

    // シャードのセットアップ（移動可能・ターゲット指定オプション付き）
    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed, Transform target)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        // 爆発タイマー開始
        Invoke(nameof(Explode), detonationTime);

        // 移動可能ならターゲットへ向かう
        if (canMove)
            MoveTowardsClosestTarget(shardSpeed, target);
    }

    // 爆発処理：範囲ダメージ、VFX生成、イベント通知
    public void Explode()
    {
        // 周囲の敵にダメージ
        DamageEnemiesInRadius(transform, checkRadius);

        // 爆発VFX生成＆エレメント色設定
        GameObject sprite = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        sprite.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);

        // 爆発イベント発火
        OnExplode?.Invoke();

        // シャードオブジェクト破棄
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵に触れた場合、即座に爆発
        if (collision.GetComponent<Entity_Enemy>() != null)
            Explode();
    }
}
