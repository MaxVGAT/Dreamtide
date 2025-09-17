using UnityEngine;

// �_���[�W��󂯂邱�Ƃ��ł���I�u�W�F�N�g�p�C���^�[�t�F�[�X
public interface IDamageable
{
    // �_���[�W��^�����Ƃ��ɌĂ΂��֐�
    // damage: �����_���[�W��
    // elementalDamage: �����_���[�W��
    // element: �����^�C�v
    // damageDealer: �_���[�W��^�����I�u�W�F�N�g��Transform
    // �߂�l: �_���[�W���K�p���ꂽ���ǂ���
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer);
}
