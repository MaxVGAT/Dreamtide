using UnityEngine;

public class Enemy_VFX : Entity_VFX // �U���^�[�Q�b�g�̂��߂̏����ȕ⏕�N���X
{
    [Header("�J�E���^�[�U���E�B���h�E��VFX")]
    [SerializeField] private GameObject attackAlert; // �U���x���p�̃Q�[���I�u�W�F�N�g����蓖�Ă�

    // �G���U����d�|����ۂɍU���x����\���E��\���ɐ؂�ւ���
    public void EnableAttackAlert(bool enable) => attackAlert.SetActive(enable);
}
