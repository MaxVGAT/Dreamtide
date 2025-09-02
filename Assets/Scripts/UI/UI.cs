using UnityEngine;

// �Q�[����UI�S�̂�Ǘ�����N���X
public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot; // �^�u���j���[�S�̂̃��[�g�I�u�W�F�N�g
    public UI_SkillTree skillTree; // �X�L���c���[UI
    public UI_ItemTooltip itemTooltip;               // �A�C�e���c�[���`�b�vUI
    public UI_StatTooltip statTooltip;               // �X�e�[�^�X�c�[���`�b�vUI

    private bool menuEnabled; // ���j���[�̕\�����

    private void Awake()
    {
        // ���j���[�������\���ɐݒ�
        tabMenuRoot.SetActive(false);

        // �q�I�u�W�F�N�g����UI�R���|�[�l���g��擾
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();
    }

    // UI�̕\���E��\����؂�ւ���
    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;

        if (tabMenuRoot != null)
            tabMenuRoot.SetActive(!menuEnabled); // ���j���[�̕\����Ԃ𔽓]

        if (itemTooltip != null)
            itemTooltip.ShowToolTip(false, null, null); // �c�[���`�b�v���\����
        else
            return;
    }
}
