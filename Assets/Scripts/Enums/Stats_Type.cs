using UnityEngine;

// �����̎��
public enum ElementType
{
    None,       // �����Ȃ�
    Fire,       // �Α���
    Ice,        // �X����
    Lightning   // ������
}

// �X�e�[�^�X�̎��
public enum StatType
{
    MaxHealth,      // �ő�̗�
    HealthRegen,    // �̗͉񕜗�
    Strength,       // ��
    Agility,        // �q��
    Intelligence,   // �m��
    Vitality,       // �̗́E�ϋv
    AttackSpeed,    // �U�����x
    Damage,         // �����_���[�W
    CritChance,     // �N���e�B�J����
    CritPower,      // �N���e�B�J���З�
    ArmorReduction, // �G�h��ђʗ�
    FireDamage,     // �Α����_���[�W
    IceDamage,      // �X�����_���[�W
    LightningDamage,// �������_���[�W
    Armor,          // �h���
    Evasion,        // ���
    IceResistance,  // �X�����ϐ�
    FireResistance, // �Α����ϐ�
    LightningResistance, // �������ϐ�
    ElementalDamage // �������_���[�W�i�v�Z�p�j
}
