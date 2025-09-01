using UnityEngine;
using UnityEngine.UI;

// 体力を管理するクラス
public class Entity_Health : MonoBehaviour, IDamageable
{
    private Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;

    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    [Header("Health Regen")] // 体力回復設定
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    public float lastDamageTaken { get; private set; }

    [Header("On Damage Knockback")] // ダメージ時のノックバック設定
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.6f;

    [Header("On Heavy damages")] // 大ダメージと判定する割合
    [SerializeField] private float heavyDamageThreshold = 0.3f;

    protected virtual void Awake()
    {
        // 必要なコンポーネントを取得
        healthBar = GetComponentInChildren<Slider>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();

        SetupHealth();
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();

        // 一定間隔でHP回復処理を呼ぶ
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    // ダメージを受ける処理
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        // すでに死んでいる、または無敵状態なら無視
        if (isDead || canTakeDamage == false)
            return false;

        // 回避に成功したら無効
        if (AttackAvoided())
            return false;

        // 攻撃者のステータスから防御力を取得
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float physicalDamageTaken, elementalDamageTaken;

        // 防御・耐性を考慮した最終ダメージを計算
        ApplyPhysAndElemRes(damage, elementalDamage, element, armorReduction, out physicalDamageTaken, out elementalDamageTaken);

        // ノックバック適用
        TakeKnockback(damageDealer, physicalDamageTaken);

        // HPを減らす
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        // 最後に受けたダメージを記録
        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;

        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    // ダメージ計算（物理・属性）
    private void ApplyPhysAndElemRes(float damage, float elementalDamage, ElementType element, float armorReduction, out float physicalDamageTaken, out float elementalDamageTaken)
    {
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0; // 物理軽減率
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;   // 属性耐性

        physicalDamageTaken = damage * (1 - mitigation);
        elementalDamageTaken = elementalDamage * (1 - resistance);
    }

    // 回避判定（回避率でランダム判定）
    private bool AttackAvoided()
    {
        if (entityStats == null)
            return false;
        else
            return Random.Range(0, 100) < entityStats.GetEvasion();
    }

    // 体力回復処理
    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue(); // 回復量を取得
        IncreaseHealth(regenAmount);
    }

    // HPを回復する
    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        // 最大値を超えないように調整
        currentHealth = Mathf.Min(newHealth, maxHealth);

        UpdateHealthBar();
    }

    // HPを減らす（0以下なら死亡処理）
    public void ReduceHealth(float damage)
    {
        entityVfx?.HandleHitColor(Entity_VFX.FlashType.Red); // ヒット時のVFX
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    // HPバー更新
    private void UpdateHealthBar()
    {
        if (healthBar == null || entityStats == null)
            return;

        float maxHealth = entityStats.GetMaxHealth();
        if (maxHealth <= 0)
            return;

        healthBar.value = Mathf.Clamp01(currentHealth / maxHealth); // 0～1に正規化
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        UpdateHealthBar();
    }

    // 死亡処理（オーバーライド可能）
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    // ノックバック処理
    private float TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateKnockbackDuration(finalDamage);

        // ガード中ならノックバックせずダメージ半減
        if (entity != null && entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow); // ガード時の色
            finalDamage /= 2;
        }
        else
        {
            entity?.ReceiveKnockback(knockback, duration); // ノックバックを適用
        }

        return finalDamage;
    }

    // ノックバック方向と強さを計算
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        // どちらの方向に飛ばすか判定（右 or 左）
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        // 大ダメージなら強いノックバック
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction; // 攻撃者の位置で方向反転

        return knockback;
    }

    // ノックバック時間を計算
    private float CalculateKnockbackDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    // 大ダメージかどうか判定
    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;
        else
            return damage / entityStats.GetMaxHealth() > heavyDamageThreshold; // ダメージ割合がしきい値以上か
    }
}
