using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour // 戦闘関連処理
{
    public event Action<float> OnDoingPhysicalDamage; // 物理ダメージ発生イベント

    private Entity_Stats stats; // ステータス参照用
    protected Entity_Stats Stats => stats; // 継承クラス用アクセス
    private Entity_SFX sfx; // サウンド参照

    private Entity_VFX vfx; // VFX参照

    public DamageScaleData basicAttackScale; // 攻撃スケールデータ

    [Header("ターゲット検出")]
    [SerializeField] private Transform targetCheck; // 判定中心
    [SerializeField] private float targetCheckRadius = 1; // 判定半径
    [SerializeField] private LayerMask whatIsTarget; // 判定対象レイヤー

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
        sfx = GetComponent<Entity_SFX>();
    }

    public void PerformAttack()
    {
        bool targetGotHit = false;

        // 判定内の対象に対して攻撃処理
        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) continue; // ダメージ可能でなければスキップ

            AttackData attackData = stats.GetAttackData(basicAttackScale); // 攻撃データ取得
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            // ダメージ処理
            targetGotHit = damageable.TakeDamage(physDamage, elementalDamage, element, transform);
            Debug.Log($"Collider hit: {target.name}, TakeDamage returned {targetGotHit}");

            // 状態異常付与
            if (element != ElementType.None)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);

            // ヒットVFX・サウンド再生
            if (targetGotHit)
            {
                OnDoingPhysicalDamage?.Invoke(physDamage);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element);
                sfx?.PlayAttackHit();
            }
        }

        if (!targetGotHit)
            sfx?.PlayAttackMiss(); // 外した場合の音再生
    }

    // 判定内のコライダー取得
    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    // 判定範囲Gizmos表示
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
