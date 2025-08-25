using UnityEngine;

public class Entity_Combat : MonoBehaviour // ターゲット検出のための基底クラス
{
    private Entity_Stats stats; // 攻撃関数で使用するステータスをキャッシュ
    protected Entity_Stats Stats => stats; // サブクラスから読み取り可能にするためのプロパティ

    private Entity_VFX vfx; // 被弾時に使うVFXスクリプトをキャッシュ

    public DamageScaleData basicAttackScale; // PerformAttack関数で使用するダメージスケールデータ

    [Header("ターゲット検出")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1; // 攻撃範囲
    [SerializeField] private LayerMask whatIsTarget; // 対象レイヤーの設定

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        // 範囲内の全ターゲットに対してダメージを与える
        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue; // 対象が無効ならスキップ

            // 必要なステータス情報を取得
            AttackData attackData = stats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            // 被弾処理：成功すればtrue
            bool targetGotHit = damageable.TakeDamage(physDamage, elementalDamage, element, transform);

            // 属性攻撃がある場合はステータス効果を付与
            if (element != ElementType.None)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);

            // 被弾した対象にヒット確認用の赤いVFXを生成
            if (targetGotHit)
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element);
        }
    }

    // 円形範囲内にいるすべてのターゲットを配列として取得
    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    // エディタ上で攻撃範囲を視覚化
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
