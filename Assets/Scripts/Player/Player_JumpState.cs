using UnityEngine;

// �v���C���[�̃W�����v��ԁi�󒆂ɂ���Ԃ̈ړ��Ǘ��j
public class Player_JumpState : PlayerAiredState
{
    public Player_JumpState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SoundManager.instance.PlaySFX("jump", player.GetComponentInChildren<AudioSource>());
        // �W�����v�J�n���̐������x��ݒ�i�����x�͈ێ��j
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        // �������Ɉړ����ŁA�W�����v�U�����łȂ���Η�����ԂɈڍs
        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }
}
