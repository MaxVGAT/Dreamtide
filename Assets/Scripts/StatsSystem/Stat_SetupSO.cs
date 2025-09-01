using UnityEngine;

// �L�����N�^�[�̃f�t�H���g�X�e�[�^�X��ۑ�����ScriptableObject
[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("����[�X")]
    public float maxHealth = 100; // �ő�̗�
    public float healthRegen;     // 1�b������̗͉̑񕜗�

    [Header("�U���� - �����_���[�W")]
    public float attackSpeed = 1; // �U�����x�i�b������̍U���񐔁j
    public float damage = 10;     // ��{�����_���[�W
    public float critChance;      // �N���e�B�J�������m��
    public float critPower = 150; // �N���e�B�J�����̃_���[�W�{���i%�j

    [Header("�U���� - �����_���[�W")]
    public float fireDamage;      // �Α����_���[�W
    public float iceDamage;       // �X�����_���[�W
    public float lightningDamage; // �������_���[�W

    [Header("�h��� - �����_���[�W")]
    public float armorReduction;  // �����_���[�W�������i%�j
    public float evasion;         // ���
    public float armor;           // �Œ�h��́i�_���[�W�����Ɋ�^�j

    [Header("�h��� - �����_���[�W")]
    public float fireResistance;      // �Α����_���[�W�ϐ��i%�j
    public float iceResistance;       // �X�����_���[�W�ϐ��i%�j
    public float lightningResistance; // �������_���[�W�ϐ��i%�j

    [Header("��v�X�e�[�^�X")]
    public float strength;     // �ʏ�A�����U���͂�ߐڃ_���[�W�ɉe��
    public float agility;      // �U�����x�A��𗦁A�ړ����x�ɉe��
    public float intelligence; // ���@�U���͂�X�L�����ʂɉe��
    public float vitality;     // �ő�̗͂�ϋv�͂ɉe��
}
