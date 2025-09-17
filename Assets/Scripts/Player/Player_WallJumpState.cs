using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    // �ǃW�����v�J�n���̏���
    public override void Enter()
    {
        base.Enter();

        SoundManager.instance.PlaySFX("wallJump", player.GetComponentInChildren<AudioSource>());
        // �v���C���[�ɕǃW�����v�̏����x��ݒ�
        player.SetVelocity(player.wallJumpDir.x * -player.facingDirection, player.wallJumpDir.y);
    }

    // ���t���[���X�V
    public override void Update()
    {
        base.Update();

        // �㏸���I������痎����Ԃ�
        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        // �ǂɐG��Ă���ΕǃX���C�h��Ԃ�
        if (player.isWallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
