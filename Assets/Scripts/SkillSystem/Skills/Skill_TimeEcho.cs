using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab; // �v���n�u�����ꂽ�^�C���G�R�[
    [SerializeField] private float timeEchoDuration;    // �^�C���G�R�[�̑��ݎ���

    [Header("Attack upgrades")]
    [SerializeField] private int maxAttacks = 3;        // �ő�U���񐔁i�}���`�A�^�b�N�p�j
    [SerializeField] private float duplicateChance = 0.3f; // �U�������̊m��

    [Header("Heal Wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = 0.3f; // �񕜗ʁi�󂯂��_���[�W�̊����j
    [SerializeField] private float cooldownReducedInSeconds;  // �N�[���_�E���Z�k��

    // Wisp�`�Ԃ̏ꍇ�ɉ񕜊�����Ԃ�
    public float GetPercentOfDamageHealed()
    {
        if (!ShouldBeWisp())
            return 0;

        return damagePercentHealed;
    }

    // Wisp�A�b�v�O���[�h���L���ȏꍇ�A�N�[���_�E���Z�k�ʂ�Ԃ�
    public float GetCooldownReduceInSeconds()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_CooldownWisp)
            return 0;

        return cooldownReducedInSeconds;
    }

    // �l�K�e�B�u���ʂ�����ł��邩����
    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    // ���݂̃A�b�v�O���[�h����/�N�����YWisp������
    public bool ShouldBeWisp()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_HealWisp
            || upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    // �����U���̊m����Ԃ�
    public float GetDuplicateChance()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 0;

        return duplicateChance;
    }

    // �ő�U���񐔂�Ԃ�
    public int GetMaxAttacks()
    {
        if (upgradeType == Skill_UpgradeType.TimeEcho_SingleAttack
            || upgradeType == Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;

        if (upgradeType == Skill_UpgradeType.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;
    }

    // �^�C���G�R�[�̑��ݎ��Ԃ�Ԃ�
    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    // �X�L���g�p����
    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        // �G�R�[��쐬����ʒu�����i���݈ʒu�j
        Vector3 exactPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        CreateTimeEcho(exactPosition);
        SetSkillOnCooldown();
    }

    // �^�C���G�R�[�𐶐�
    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position; // �w�肪�Ȃ���Ό��݈ʒu

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this); // �Z�b�g�A�b�v
    }
}
