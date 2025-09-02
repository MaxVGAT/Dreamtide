public abstract class PlayerState : EntityState
{
    protected Entity_Player player;           // �v���C���[�{�̂ւ̎Q��
    protected PlayerInputSet input;           // �v���C���[�̓��͏��
    protected Player_SkillManager skillManager; // �v���C���[�̃X�L���Ǘ�

    // �R���X�g���N�^�F�v���C���[�ƃX�e�[�g�}�V���A�A�j���[�V�������������
    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillManager = player.skillManager;
    }

    // ���t���[���X�V
    public override void Update()
    {
        base.Update();

        // �_�b�V�����͏���
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCooldown(); // �_�b�V���X�L����N�[���_�E���ɐݒ�
            stateMachine.ChangeState(player.dashState); // �_�b�V���X�e�[�g�֐؂�ւ�
        }

        // �A���e�B���b�g�X�L�����͏���
        if (input.Player.UltimateSkill.WasPressedThisFrame() && skillManager.domain.CanUseSkill())
        {
            if (skillManager.domain.InstantDomain()) // ���������\�Ȃ�
            {
                skillManager.domain.CreateDomain();   // �h���C���𐶐�
            }
            else
                stateMachine.ChangeState(player.domainState); // �����łȂ���΃X�e�[�g�؂�ւ�

            skillManager.domain.SetSkillOnCooldown(); // �X�L����N�[���_�E���ɐݒ�
        }
    }

    // �A�j���[�V�����p�����[�^�X�V
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y); // Y�����x��A�j���[�^�[�ɔ��f
    }

    // �_�b�V���\������
    private bool CanDash()
    {
        if (!skillManager.dash.CanUseSkill())   // �X�L���g�p�s��
            return false;

        if (player.isWallDetected)               // �ǐڐG���͕s��
            return false;

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainState)
            return false;                        // ���Ƀ_�b�V������h���C�����͕s��

        return true;
    }
}
