using System.Linq;
using TMPro;
using UnityEngine;

// �X�L���c���[UI��Ǘ�����N���X
public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private UI_SkillTooltip skillTooltip; // �X�L���̏ڍ׃c�[���`�b�vUI         // �v���C���[�̎c��X�L���|�C���g
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes; // �ڑ��n���h���[�z��
    private UI_TreeNode[] allTreeNodes;

    public Player_SkillManager skillManager { get; private set; } // �v���C���[�̃X�L���Ǘ��N���X�Q��

    private void Awake()
    {
        // �V�[�������Player_SkillManager��������Ď擾
        skillManager = FindAnyObjectByType<Player_SkillManager>();
        skillManager.OnSkillPointsChanged += UpdateSkillPointsText;

        UpdateSkillPointsText(skillManager.skillPoints);

        allTreeNodes = GetComponentsInChildren<UI_TreeNode>();

    }

    private void Start()
    {
        // �X�L���c���[�̐ڑ���Ԃ�X�V
        UpdateAllConnections();
    }

    private void UpdateSkillPointsText(int points)
    {
        if (skillPointsText != null)
            skillPointsText.text = skillManager.skillPoints.ToString();
    }

    // �R���e�L�X�g���j���[����S�X�L������Z�b�g
    [ContextMenu("Refund all skills")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund(); // �e�m�[�h�̃X�L����ԋp
    }

    // �X�L���c�[���`�b�v��擾
    public UI_SkillTooltip SkillTooltip => skillTooltip;

    // �X�L���|�C���g������邩����
    public bool EnoughSkillPoints(int cost) => skillManager.skillPoints >= cost;

    // �X�L���|�C���g�����
    public void RemoveSkillPoint(int cost)
    {
        if(skillManager.SpendSkillPoints(cost))
            UpdateSkillPointsText(skillManager.skillPoints);
    }

    // �X�L���|�C���g��ǉ�
    public void AddSkillPoints(int points)
    {
        skillManager.AddSkillPoints(points);
        UpdateSkillPointsText(skillManager.skillPoints);
    }

    // �S�Ă̐e�m�[�h�̐ڑ���X�V
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }

    public void LoadData(GameData data)
    {
        skillManager.skillPoints = data.skillPoints;
        UpdateSkillPointsText(skillManager.skillPoints);

        // Validate before unlocking
        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
            {
                bool canUnlock = true;

                foreach (var neededNode in node.neededNodes)
                {
                    if (!data.skillTreeUI.TryGetValue(neededNode.skillData.skillName, out bool parentNodeUnlocked) || !parentNodeUnlocked)
                    {
                        canUnlock = false;
                        break;
                    }
                }

                if (canUnlock)
                {
                    foreach (var conflictNode in node.conflictNodes)
                    {
                        if (data.skillTreeUI.TryGetValue(conflictNode.skillData.skillName, out bool conflictUnlocked) && conflictUnlocked)
                        {
                            canUnlock = false;
                            break;
                        }
                    }
                }

                if (canUnlock)
                {
                    node.UnlockWithSaveData();
                }

                else
                {
                    data.skillTreeUI[skillName] = false;
                }
            }
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out Skill_UpgradeType upgradeType))
            {
                var upgradeNode = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);

                if (upgradeNode != null && upgradeNode.isUnlocked)
                {
                    skill.SetSkillUpgrade(upgradeNode.skillData);
                }
                else
                    data.skillUpgrades.Remove(skill.GetSkillType());
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.skillPoints = skillManager.skillPoints;

        data.skillTreeUI.Clear();
        data.skillUpgrades.Clear();

        foreach(var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach(var skill in skillManager.allSkills)
        {
            data.skillUpgrades[skill.GetSkillType()] = skill.GetUpgrade();
        }
    }
}
