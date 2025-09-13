using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Entity_Player player { get; private set; }             // �����v���C���[
    public Player_SkillManager skillManager { get; private set; } // �X�L���Ǘ��R���|�[�l���g
    public DamageScaleData damageScaleData { get; private set; }  // �X�L���̃_���[�W�{���E�������

    [Header("General details")]
    [SerializeField] protected Skill_Type skillType;             // �X�L���̎��
    [SerializeField] protected Skill_UpgradeType upgradeType;    // �X�L���̃A�b�v�O���[�h���
    [SerializeField] protected float cooldown;                  // �N�[���_�E������
    private float lastTimeUsed;                                   // �Ō�Ɏg�p��������

    // ����������
    protected virtual void Awake()
    {
        player = GetComponentInParent<Entity_Player>();         // �e����v���C���[�Q��
        skillManager = GetComponentInParent<Player_SkillManager>(); // �X�L���}�l�[�W���Q��
        lastTimeUsed = lastTimeUsed - cooldown;                // ����g�p�\��Ԃɂ���
        damageScaleData = new DamageScaleData();               // �_���[�W��񏉊���
    }

    // �X�L���g�p����݂�i�I�[�o�[���C�h�p�j
    public virtual void TryUseSkill()
    {
        // �q�N���X�ŏ���
    }

    // �X�L���A�b�v�O���[�h��K�p
    public void SetSkillUpgrade(Skill_DataSO skillData)
    {
        UpgradeData upgrade = skillData.upgradeData;
        upgradeType = upgrade.upgradeType;// �A�b�v�O���[�h�^�C�v�ݒ�
        cooldown = upgrade.cooldown;                           // �N�[���_�E���ݒ�
        damageScaleData = upgrade.damageScaleData;            // �_���[�W���ݒ�

        player.ui.inGameUI.GetSkillSlot(skillType).SetupSkillSlot(skillData);
        ResetCooldown();                                       // �N�[���_�E�����Z�b�g
    }

    // �X�L�����g�p�\������
    public virtual bool CanUseSkill()
    {
        if (upgradeType == Skill_UpgradeType.None)
            return false;                                     // �X�L�����擾�Ȃ�s��

        if (OnCooldown())
            return false;                                     // �N�[���_�E�����͕s��

        // TODO: ��������}�i�ʔ���Ȃǒǉ��\

        return true;
    }

    // ����̃A�b�v�O���[�h������Ă��邩
    protected bool Unlocked(Skill_UpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;
    public Skill_UpgradeType GetUpgrade() => upgradeType;
    public Skill_Type GetSkillType() => skillType;

    // �N�[���_�E����������
    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    // �g�p���ɃN�[���_�E����ݒ�
    public void SetSkillOnCooldown()
    {
        player.ui.inGameUI.GetSkillSlot(skillType).StartCooldown(cooldown);
        lastTimeUsed = Time.time;
    }

    // �N�[���_�E����Z�k
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;

    // �N�[���_�E������Z�b�g���đ��g�p�\�ɂ���
    public void ResetCooldown()
    {
        player.ui.inGameUI.GetSkillSlot(skillType).ResetCooldown();
        lastTimeUsed = Time.time - cooldown;
    }
}
