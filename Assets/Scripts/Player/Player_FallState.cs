// �v���C���[�̗������
public class Player_FallState : PlayerAiredState
{
    public Player_FallState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }

        else if (player.isWallDetected && rb.linearVelocity.y <= 0)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
