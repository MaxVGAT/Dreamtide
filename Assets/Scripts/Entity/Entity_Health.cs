using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// エンティティのHP管理
public class Entity_Health : MonoBehaviour, IDamageable
{
    public event Action OnTakingDamage; // ダメージ時イベント
    public event Action OnHealthUpdate; // HP更新時イベント

    private Slider healthBar; // UIスライダー
    private Entity_VFX entityVfx; // VFX参照
    private Entity entity; // エンティティ本体
    private Entity_Stats entityStats; // ステータス参照
    private Entity_DropManager dropManager; // アイテムドロップ管理

    private bool miniHealthBarActive;

    [SerializeField] protected float currentHealth; // 現在HP

    public bool isDead { get; private set; } // 死亡判定
    protected bool canTakeDamage = true; // ダメージ受け取り可否

    [Header("Health Regen")] // HP自動回復設定
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    public float lastDamageTaken { get; private set; } // 最終ダメージ

    [Header("On Damage Knockback")] // ダメージ時ノックバック設定
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.6f;

    [Header("On Heavy damages")] // 重ダメージ閾値
    [SerializeField] private float heavyDamageThreshold = 0.3f;

    protected virtual void Awake()
    {
        // コンポーネント取得
        healthBar = GetComponentInChildren<Slider>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();

        SetupHealth();
    }

    private void SetupHealth()
    {
        if (entityStats == null)
            return;

        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();

        // 定期回復開始
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    // ダメージ処理
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead || !canTakeDamage) return false; // 死亡中または無敵中は無効
        if (AttackAvoided()) return false; // 回避判定

        // 攻撃者の防御値取得
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float physicalDamageTaken, elementalDamageTaken;

        // 物理・属性耐性適用
        ApplyPhysAndElemRes(damage, elementalDamage, element, armorReduction, out physicalDamageTaken, out elementalDamageTaken);

        // ノックバック処理
        TakeKnockback(damageDealer, physicalDamageTaken);

        // HP減少
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;

        OnTakingDamage?.Invoke();
        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    // 物理・属性耐性を反映した実ダメージ計算
    private void ApplyPhysAndElemRes(float damage, float elementalDamage, ElementType element, float armorReduction, out float physicalDamageTaken, out float elementalDamageTaken)
    {
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;

        physicalDamageTaken = damage * (1 - mitigation);
        elementalDamageTaken = elementalDamage * (1 - resistance);
    }

    public float GetCurrentHealth() => currentHealth;

    // 攻撃回避判定
    private bool AttackAvoided()
    {
        return entityStats != null && UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    // 自動回復
    private void RegenerateHealth()
    {
        if (!canRegenerateHealth) return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    // HP回復
    public void IncreaseHealth(float healAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, entityStats.GetMaxHealth());
        OnHealthUpdate?.Invoke();
    }

    // HP減少
    public void ReduceHealth(float damage)
    {
        currentHealth -= damage;
        entityVfx?.HandleHitColor(Entity_VFX.FlashType.Red); // 被ダメVFX
        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0) Die();
    }

    // HPバー更新
    private void UpdateHealthBar()
    {
        if (healthBar == null || healthBar.transform.parent.gameObject.activeSelf == false) return;

        float maxHealth = entityStats.GetMaxHealth();
        if (maxHealth <= 0) return;

        healthBar.value = Mathf.Clamp01(currentHealth / maxHealth);
    }

    public void EnableHealthBar(bool enable) => healthBar?.transform.parent.gameObject.SetActive(enable);
    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        OnHealthUpdate?.Invoke();
    }

    // 死亡処理
    protected virtual void Die()
    {
        isDead = true;
        entity?.EntityDeath(); // エンティティ死亡処理
        dropManager?.DropItems(); // アイテムドロップ
    }

    // ノックバック処理
    private float TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateKnockbackDuration(finalDamage);

        if (entity != null && entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow); // ブロック時VFX
            finalDamage /= 2;
        }
        else
        {
            entity?.ReceiveKnockback(knockback, duration); // 通常ノックバック
        }

        return finalDamage;
    }

    // ノックバック量計算
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;
        return knockback;
    }

    // ノックバック時間計算
    private float CalculateKnockbackDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    // 重ダメージ判定
    private bool IsHeavyDamage(float damage)
    {
        return entityStats != null && damage / entityStats.GetMaxHealth() > heavyDamageThreshold;
    }
}
