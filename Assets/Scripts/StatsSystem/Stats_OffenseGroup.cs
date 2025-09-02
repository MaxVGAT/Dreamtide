using UnityEngine;
using System;

[Serializable]
// �U���X�e�[�^�X�i�I�t�F���X�n�\�͒l�j��܂Ƃ߂��N���X
public class Stats_OffenseGroup
{
    public Stats attackSpeed;   // �U�����x�F�U���̊Ԋu�ɉe��

    // �����U���֘A
    public Stats damage;        // ��{�_���[�W
    public Stats critPower;     // �N���e�B�J�����̃_���[�W�{��
    public Stats critChance;    // �N���e�B�J��������
    public Stats armorReduction; // �G�̖h��͌�����

    // �����U���֘A
    public Stats fireDamage;    // �Α����_���[�W
    public Stats iceDamage;     // �X�����_���[�W
    public Stats lightningDamage; // �������_���[�W
    public Stats elementalDamage;
}
