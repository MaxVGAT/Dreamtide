using UnityEngine;

// �v���C���[�̒n��ҋ@��ԁiIdle�j
public class Player_IdleState : PlayerGroundedState
{
    public Player_IdleState(Entity_Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.input.Enable();

        player.rb.simulated = true;
        // �ҋ@���͉������̑��x��[���ɐݒ�
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // �v���C���[���ǂɌ������ē��͂��Ă���ꍇ�͓����Ȃ�
        if (player.moveInput.x == player.facingDirection && player.isWallDetected)
            return;

        // ���������͂�����ꍇ�͈ړ���ԂɑJ��
        if (player.moveInput.x != 0)
            stateMachine.ChangeState(player.moveState);
    }
}
