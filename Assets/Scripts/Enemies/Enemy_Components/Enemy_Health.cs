using UnityEngine;

public class Enemy_Health : Entity_Health // ���ׂĂ̗̑͂���G���e�B�e�B�̊�{�N���X
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>(); // �_���[�W�����̂��߂�Entity_Enemy�R���|�[�l���g��擾

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canTakeDamage == false)
            return false;

        // �G���U����󂯂����m�F���A�_���[�W��K�p�B���̌�A�\�Ȃ�i�퓬��ԂłȂ���΁j�퓬��Ԃֈڍs
        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        // �G�����S���Ă���ΐ퓬��Ԃɓ���K�v���Ȃ��̂ŏ�����I��
        if (isDead)
            return false;

        // �_���[�W��^�����̂��v���C���[�Ȃ�A�G�͐퓬��Ԃֈڍs����݂�
        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }
}
