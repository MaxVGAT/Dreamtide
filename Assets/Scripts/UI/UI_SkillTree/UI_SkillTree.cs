using UnityEngine;

// �X�L���c���[UI��Ǘ�����N���X
public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private UI_SkillTooltip skillTooltip; // �X�L���̏ڍ׃c�[���`�b�vUI
    [SerializeField] private int skillPoints;             // �v���C���[�̎c��X�L���|�C���g
    [SerializeField] private UI_TreeConnectHandler[] parentNodes; // �ڑ��n���h���[�z��

    public Player_SkillManager skillManager { get; private set; } // �v���C���[�̃X�L���Ǘ��N���X�Q��

    private void Awake()
    {
        // �V�[�������Player_SkillManager��������Ď擾
        skillManager = FindAnyObjectByType<Player_SkillManager>();
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
    public bool EnoughSkillPoints(int cost) => skillManager.SkillPoints >= cost;

    // �X�L���|�C���g�����
    public void RemoveSkillPoint(int cost) => skillManager.SpendSkillPoints(cost);

    // �X�L���|�C���g��ǉ�
    public void AddSkillPoints(int points) => skillManager.AddSkillPoints(points);

    private void Start()
    {
        // �X�L���c���[�̐ڑ���Ԃ�X�V
        UpdateAllConnections();
    }

    // �S�Ă̐e�m�[�h�̐ڑ���X�V
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }
}
