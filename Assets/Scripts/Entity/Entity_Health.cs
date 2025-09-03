using System;
using UnityEngine;
using UnityEngine.UI;

// �̗͂�Ǘ�����N���X
public class Entity_Health : MonoBehaviour, IDamageable
{
    public event Action OnTakingDamage;

    private Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats entityStats;

    [SerializeField] protected float currentHealth;

    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    [Header("Health Regen")] // �̗͉񕜐ݒ�
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;
    public float lastDamageTaken { get; private set; }

    [Header("On Damage Knockback")] // �_���[�W���̃m�b�N�o�b�N�ݒ�
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.6f;

    [Header("On Heavy damages")] // ��_���[�W�Ɣ��肷�銄��
    [SerializeField] private float heavyDamageThreshold = 0.3f;

    protected virtual void Awake()
    {
        // �K�v�ȃR���|�[�l���g��擾
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

        // ���Ԋu��HP�񕜏�����Ă�
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    // �_���[�W��󂯂鏈��
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        // ���łɎ���ł���A�܂��͖��G��ԂȂ疳��
        if (isDead || canTakeDamage == false)
            return false;

        // ���ɐ��������疳��
        if (AttackAvoided())
            return false;

        // �U���҂̃X�e�[�^�X����h��͂�擾
        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float physicalDamageTaken, elementalDamageTaken;

        // �h��E�ϐ���l�������ŏI�_���[�W��v�Z
        ApplyPhysAndElemRes(damage, elementalDamage, element, armorReduction, out physicalDamageTaken, out elementalDamageTaken);

        // �m�b�N�o�b�N�K�p
        TakeKnockback(damageDealer, physicalDamageTaken);

        // HP����炷
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        // �Ō�Ɏ󂯂��_���[�W��L�^
        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;

        OnTakingDamage?.Invoke();
        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    // �_���[�W�v�Z�i�����E�����j
    private void ApplyPhysAndElemRes(float damage, float elementalDamage, ElementType element, float armorReduction, out float physicalDamageTaken, out float elementalDamageTaken)
    {
        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0; // �����y����
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;   // �����ϐ�

        physicalDamageTaken = damage * (1 - mitigation);
        elementalDamageTaken = elementalDamage * (1 - resistance);
    }

    // ��𔻒�i��𗦂Ń����_������j
    private bool AttackAvoided()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    // �̗͉񕜏���
    private void RegenerateHealth()
    {
        if (canRegenerateHealth == false)
            return;

        float regenAmount = entityStats.resources.healthRegen.GetValue(); // �񕜗ʂ�擾
        IncreaseHealth(regenAmount);
    }

    // HP��񕜂���
    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        // �ő�l�𒴂��Ȃ��悤�ɒ���
        currentHealth = Mathf.Min(newHealth, maxHealth);

        UpdateHealthBar();
    }

    // HP����炷�i0�ȉ��Ȃ玀�S�����j
    public void ReduceHealth(float damage)
    {
        entityVfx?.HandleHitColor(Entity_VFX.FlashType.Red); // �q�b�g����VFX
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    // HP�o�[�X�V
    private void UpdateHealthBar()
    {
        if (healthBar == null || entityStats == null)
            return;

        float maxHealth = entityStats.GetMaxHealth();
        if (maxHealth <= 0)
            return;

        healthBar.value = Mathf.Clamp01(currentHealth / maxHealth); // 0�`1�ɐ��K��
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        UpdateHealthBar();
    }

    // ���S�����i�I�[�o�[���C�h�\�j
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    // �m�b�N�o�b�N����
    private float TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateKnockbackDuration(finalDamage);

        // �K�[�h���Ȃ�m�b�N�o�b�N�����_���[�W����
        if (entity != null && entity.isBlocking)
        {
            entityVfx.HandleHitColor(Entity_VFX.FlashType.Yellow); // �K�[�h���̐F
            finalDamage /= 2;
        }
        else
        {
            entity?.ReceiveKnockback(knockback, duration); // �m�b�N�o�b�N��K�p
        }

        return finalDamage;
    }

    // �m�b�N�o�b�N�����Ƌ�����v�Z
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        // �ǂ���̕����ɔ�΂�������i�E or ���j
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        // ��_���[�W�Ȃ狭���m�b�N�o�b�N
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction; // �U���҂̈ʒu�ŕ������]

        return knockback;
    }

    // �m�b�N�o�b�N���Ԃ�v�Z
    private float CalculateKnockbackDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    // ��_���[�W���ǂ�������
    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
            return false;
        else
            return damage / entityStats.GetMaxHealth() > heavyDamageThreshold; // �_���[�W�������������l�ȏォ
    }
}
