using UnityEngine;
using System;

// �v���C���[�̃X�L���Ǘ��N���X
// �e�X�L���ւ̎Q�Ƃ�ێ����A�N�[���_�E���Ǘ���X�L���擾���
public class Player_SkillManager : MonoBehaviour
{
    public event Action<int>OnSkillPointsChanged;

    // �e�X�L���̃v���p�e�B
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_Domain domain { get; private set; }

    public Skill_Base[] allSkills { get; private set; } // �S�X�L���̔z��i���ʏ����p�j

    public int skillPoints;


    private void Awake()
    {
        // �q�I�u�W�F�N�g����e�X�L���R���|�[�l���g��擾
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        domain = GetComponentInChildren<Skill_Domain>();

        // �S�X�L����z��Ƃ��Ď擾�i���ʏ����p�j
        allSkills = GetComponentsInChildren<Skill_Base>();
    }

    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    public bool SpendSkillPoints(int cost)
    {
        if (skillPoints >= cost)
        {
            skillPoints -= cost;
            OnSkillPointsChanged?.Invoke(skillPoints);
            return true;
        }
        return false;
    }

    // �S�X�L���̃N�[���_�E�������������
    public void ReduceAllSkillsBooldownBy(float amount)
    {
        foreach (var skills in allSkills)
            skills.ReduceCooldownBy(amount);
    }

    // �X�L���^�C�v����X�L����擾
    public Skill_Base GetSkillByType(Skill_Type type)
    {
        switch (type)
        {
            case Skill_Type.Dash:
                return dash;
            case Skill_Type.TimeShard:
                return shard;
            case Skill_Type.SwordThrow:
                return swordThrow;
            case Skill_Type.TimeEcho:
                return timeEcho;
            case Skill_Type.Domain:
                return domain;
            default:
                Debug.Log("Not implemented");
                return null;
        }
    }
}
