using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed;   // ���̃^�[�Q�b�g�ɒ��˂鑬�x
    private int bounceCount;                       // �c��̒��ˉ�

    private Collider2D[] enemyTargets;            // �X�L���͈͓�̓G
    private Transform nextTarget;                  // ���̒��˂�Ώ�
    private List<Transform> selectedBefore = new List<Transform>(); // �ȑO�I�΂ꂽ�^�[�Q�b�g�L�^

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        transform.right = rb.linearVelocity; // ���̌�����ړ������ɍ��킹��
        HandleComeback();                    // �v���C���[�ւ̖߂菈��
        HandleBounce();                      // ���̓G�ւ̒��ˏ���
    }

    // ���̃^�[�Q�b�g�ւ̈ړ��E�U������
    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1); // �Փˎ��Ƀ_���[�W

            enemyTargets = GetEnemiesAround(transform, 10); // ���͂̓G��X�V
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBackToPlayer(); // ���ˏI���Ńv���C���[�ɖ߂�
            }
        }
    }

    // ���̃^�[�Q�b�g�����
    private void BounceToNextTarget()
    {
        Transform target = GetNextTarget();
        if (target != null)
        {
            nextTarget = target;
            bounceCount--;
        }
        else
            nextTarget = null;
    }

    // �����蔻��ɓ������Ƃ��̏���
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        anim?.SetTrigger("spin"); // ��]�A�j���[�V����

        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10); // �͈͓�̓G��擾
            rb.simulated = false;                            // �ړ���~
        }

        DamageEnemiesInRadius(transform, 1); // �͈͍U��

        // ���˂�Ώۂ����Ȃ��A�܂��͒��ˉ񐔏I��
        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBackToPlayer();
        else
            nextTarget = GetNextTarget();
    }

    // alive�ȓG������Ԃ�
    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }

    // �ߋ��ɑI�΂�Ă��Ȃ��G������Ԃ��B���ׂđI�΂ꂽ�ꍇ�̓��X�g����Z�b�g
    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && !selectedBefore.Contains(enemy.transform))
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)
            return validTargets;

        selectedBefore.Clear();
        return aliveTargets;
    }

    // �����_���Ɏ��̃^�[�Q�b�g����肵��selectedBefore�ɋL�^
    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();

        if (validTarget.Count == 0)
            return null;

        int randomIndex = Random.Range(0, validTarget.Count);
        Transform nextTarget = validTarget[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }
}
