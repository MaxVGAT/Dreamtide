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

        // �ڕW������ꍇ�A��葬�x�ňړ�
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // �w��^�[�Q�b�g�i������͍ł�߂��G�j�Ɍ������Ĉړ��J�n
    public void MoveTowardsClosestTarget(float speed, Transform newTarget = null)
{
    // Fixed logic: use newTarget if provided, otherwise find closest
    target = newTarget != null ? newTarget : FindClosestTarget();
    this.speed = speed;
}

    // �V���[�h�̊�{�Z�b�g�A�b�v�i���������̂݁j
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        // �U���v�Z�p�̏���擾
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        // �w�莞�Ԍ�Ɏ�������
        float detonationTime = shardManager.GetDetonateTime();
        Invoke(nameof(Explode), detonationTime);
    }

    // �V���[�h�̃Z�b�g�A�b�v�i�ړ��\�E�^�[�Q�b�g�w��I�v�V�����t���j
    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed, Transform target)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        // �����^�C�}�[�J�n
        Invoke(nameof(Explode), detonationTime);

        // �ړ��\�Ȃ�^�[�Q�b�g�֌�����
        if (canMove)
            MoveTowardsClosestTarget(shardSpeed, target);
    }

    // ���������F�͈̓_���[�W�AVFX�����A�C�x���g�ʒm
    public void Explode()
    {
        // ���͂̓G�Ƀ_���[�W
        DamageEnemiesInRadius(transform, checkRadius);

        // ����VFX�������G�������g�F�ݒ�
        GameObject sprite = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        sprite.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);

        // �����C�x���g����
        OnExplode?.Invoke();

        // �V���[�h�I�u�W�F�N�g�j��
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �G�ɐG�ꂽ�ꍇ�A�����ɔ���
        if (collision.GetComponent<Entity_Enemy>() != null)
            Explode();
    }
}
