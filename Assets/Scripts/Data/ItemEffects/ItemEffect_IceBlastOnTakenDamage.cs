using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Ice Blast", fileName = "Item Effect data - Ice Blast On Taken Damage")]
public class ItemEffect_IceBlastOnTakenDamage : Item_EffectDataSO
{
    [SerializeField] private ElementalEffectData effectData;
    [SerializeField] private float iceDamage;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [Header("Trigger details")]
    [SerializeField] private float healthPercentTrigger = 0.50f;
    [SerializeField] private float cooldown;
    private float lastTimeUsed = 0;

    [Header("VFX Details")]
    [SerializeField] GameObject iceBlastVfx;
    [SerializeField] private GameObject onHitVfx;

    private void OnEnable()
    {
        lastTimeUsed = -999;
        player = null;
    }

    public override void ExecuteEffect(Entity_Player player)
    {
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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy); 

        foreach(var target in enemies)
        {
            IDamageable  damageable = target.GetComponent<IDamageable>();

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
        base.Subscribe(player);
        lastTimeUsed = -999;

        player.health.OnTakingDamage += OnPlayerDamaged;
    }

    public override void Unsubscribe()
    {
        player.health.OnTakingDamage -= OnPlayerDamaged;
        player = null;
    }

    private void OnPlayerDamaged()
    {
        if (player == null) return;

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
    
