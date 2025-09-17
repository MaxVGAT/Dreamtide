using UnityEngine;

// �v���C���[�̃_�b�V�����
public class Player_DashState : PlayerState
{
    private float originalGravityScale; // �_�b�V���O�̏d�͒l
    private int dashDirection;           // �_�b�V������ (-1:��, 1:�E)

    public Player_DashState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // �_�b�V���J�n���̃G�t�F�N�g
        skillManager.dash.OnStartEffect();
        player.vfx.DoImageEchoEffect(player.dashDuration);

        SoundManager.instance.PlaySFX("dash", player.GetComponentInChildren<AudioSource>());

        // ���͂ɉ������_�b�V������
        dashDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        // �_�b�V�����Ԃ̐ݒ�
        stateTimer = player.dashDuration;

        // �d�͖�����
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        // ���G��Ԃɂ���
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        CancelDashIfNeeded();

        // �_�b�V�����̈ړ�
        player.SetVelocity(player.dashSpeed * dashDirection, 0);

        // �_�b�V���I������
        if (stateTimer < 0)
        {
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // �_�b�V���I�����̃G�t�F�N�g
        skillManager.dash.OnEndEffect();

        // �ړ��Əd�͂���ɖ߂�
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;

        // �_���[�W��󂯂����Ԃɖ߂�
        player.health.SetCanTakeDamage(true);
    }

    // �ǂɓ���������_�b�V����L�����Z��
    private void CancelDashIfNeeded()
    {
        if (player.isWallDetected)
        {
            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
