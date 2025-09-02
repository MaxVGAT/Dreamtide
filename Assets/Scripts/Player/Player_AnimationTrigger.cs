using UnityEngine;

// �v���C���[�p�A�j���[�V�����g���K�[
public class Player_AnimationTrigger : Entity_AnimationTriggers
{
    private Entity_Player player; // �e�v���C���[�Q��

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Entity_Player>(); // �e����v���C���[�擾
    }

    // �A�j���[�V�����C�x���g�p: ���𓊂���
    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();
}
