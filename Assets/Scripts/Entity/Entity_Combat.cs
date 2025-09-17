using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour // �^�[�Q�b�g���o�̂��߂̊��N���X
{
    public event Action<float> OnDoingPhysicalDamage;

    private Entity_Stats stats; // �U���֐��Ŏg�p����X�e�[�^�X��L���b�V��
    protected Entity_Stats Stats => stats; // �T�u�N���X����ǂݎ��\�ɂ��邽�߂̃v���p�e�B
    private Entity_SFX sfx;

    private Entity_VFX vfx; // ��e���Ɏg��VFX�X�N���v�g��L���b�V��

    public DamageScaleData basicAttackScale; // PerformAttack�֐��Ŏg�p����_���[�W�X�P�[���f�[�^

    [Header("�^�[�Q�b�g���o")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1; // �U���͈�
    [SerializeField] private LayerMask whatIsTarget; // �Ώۃ��C���[�̐ݒ�

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
        sfx = GetComponent<Entity_SFX>();
    }

    public void PerformAttack()
    {
        bool targetGotHit = false;

        // �͈͓�̑S�^�[�Q�b�g�ɑ΂��ă_���[�W��^����
        foreach (var target in GetDetectedColliders())
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue; // �Ώۂ������Ȃ�X�L�b�v

            // �K�v�ȃX�e�[�^�X����擾
            AttackData attackData = stats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            // ��e�����F���������true
            targetGotHit = damageable.TakeDamage(physDamage, elementalDamage, element, transform);

            Debug.Log($"Collider hit: {target.name}, TakeDamage returned {targetGotHit}");

            // �����U��������ꍇ�̓X�e�[�^�X���ʂ�t�^
            if (element != ElementType.None)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);

            // ��e�����ΏۂɃq�b�g�m�F�p�̐Ԃ�VFX�𐶐�
            if (targetGotHit)
            {
                OnDoingPhysicalDamage?.Invoke(physDamage);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit, element);
                sfx?.PlayAttackHit();
                
            }
        }

        if (targetGotHit == false)
            sfx?.PlayAttackMiss();
    }

    // �~�`�͈͓�ɂ��邷�ׂẴ^�[�Q�b�g��z��Ƃ��Ď擾
    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    // �G�f�B�^��ōU���͈͂���o��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
