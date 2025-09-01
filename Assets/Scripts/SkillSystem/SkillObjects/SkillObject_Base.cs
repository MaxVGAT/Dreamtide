using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject onHitVfx; // ヒット時のVFX
    [SerializeField] protected LayerMask whatIsEnemy; // 敵判定レイヤー
    [SerializeField] protected Transform targetCheck; // 範囲判定用の中心
    [SerializeField] protected float checkRadius = 1f; // 範囲判定半径

    protected Entity_Stats playerStats; // 攻撃者のステータス
    protected DamageScaleData damageScaleData; // ダメージスケール情報
    protected ElementType usedElement; // 使用された属性
    protected bool targetGotHit; // 対象がヒットしたか
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform lastTarget; // 最後にヒットしたターゲット

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 指定範囲内の敵にダメージ適用
    /// </summary>
    protected void DamageEnemiesInRadius(Transform t, float radius)
    {
        Collider2D[] enemies = GetEnemiesAround(t, radius);

        foreach (var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            // ダメージ適用
            targetGotHit = damageable.TakeDamage(
                attackData.physicalDamage,
                attackData.elementalDamage,
                attackData.element,
                transform
            );

            // ステータス効果適用
            if (attackData.element != ElementType.None)
                statusHandler.ApplyStatusEffect(attackData.element, attackData.effectData);

            // ヒット時VFX生成
            if (targetGotHit)
            {
                lastTarget = target.transform;
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
            }

            usedElement = attackData.element;
        }
    }

    /// <summary>
    /// 範囲内の敵を取得
    /// </summary>
    protected Collider2D[] GetEnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    /// <summary>
    /// 指定距離内で最も近い敵を返す
    /// </summary>
    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in GetEnemiesAround(transform, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }

        return target;
    }

    /// <summary>
    /// エディタ上で範囲表示
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
