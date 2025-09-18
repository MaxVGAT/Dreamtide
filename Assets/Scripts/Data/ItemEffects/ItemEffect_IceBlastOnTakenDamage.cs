using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Ice Blast", fileName = "Item Effect data - Ice Blast On Taken Damage")]
public class ItemEffect_IceBlastOnTakenDamage : Item_EffectDataSO
{
    [SerializeField] private ElementalEffectData effectData; // 付与する氷属性効果データ
    [SerializeField] private float iceDamage;               // 氷ダメージ量
    [SerializeField] private LayerMask whatIsEnemy;         // 攻撃対象レイヤー

    [Space]
    [Header("Trigger details")]
    [SerializeField] private float healthPercentTrigger = 0.50f; // 発動HP割合
    [SerializeField] private float cooldown;                     // クールタイム
    private float lastTimeUsed = 0;                               // 最終使用時間

    [Header("VFX Details")]
    [SerializeField] GameObject iceBlastVfx; // 発動エフェクト
    [SerializeField] private GameObject onHitVfx; // 命中時エフェクト

    private void OnEnable()
    {
        // 初期化
        lastTimeUsed = -999;
        player = null;
    }

    public override void ExecuteEffect(Entity_Player player)
    {
        // クールタイムとHP条件をチェックして発動
        bool noCooldown = Time.time >= lastTimeUsed + cooldown;
        bool reachedThreshold = player.health.GetHealthPercent() <= healthPercentTrigger;

        if (noCooldown && reachedThreshold)
        {
            player.vfx.CreateEffectOf(iceBlastVfx, player.transform);
            lastTimeUsed = Time.time;
            DamageEnemiesWithIce();
        }
    }

    private void DamageEnemiesWithIce()
    {
        // プレイヤー周囲の敵を検出してダメージ・状態異常適用
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);

        foreach (var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) continue;

            bool targetGotHit = damageable.TakeDamage(0, iceDamage, ElementType.Ice, player.transform);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);

            if (targetGotHit)
                player.vfx.CreateEffectOf(onHitVfx, target.transform);
        }
    }

    public override void Subscribe(Entity_Player player)
    {
        // ダメージ受けイベントに登録
        base.Subscribe(player);
        lastTimeUsed = -999;
        player.health.OnTakingDamage += OnPlayerDamaged;
    }

    public override void Unsubscribe()
    {
        // イベント解除
        player.health.OnTakingDamage -= OnPlayerDamaged;
        player = null;
    }

    private void OnPlayerDamaged()
    {
        if (player == null) return;

        // クールタイムとHP条件を再チェックして発動
        bool noCooldown = Time.time >= lastTimeUsed + cooldown;
        bool reachedThreshold = player.health.GetHealthPercent() <= healthPercentTrigger;

        if (noCooldown && reachedThreshold)
        {
            player.vfx.CreateEffectOf(iceBlastVfx, player.transform);
            lastTimeUsed = Time.time;
            DamageEnemiesWithIce();
        }
    }
}
