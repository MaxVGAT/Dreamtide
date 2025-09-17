using UnityEngine;

// �v���C���[�̈ړ���ԁi�n�ʂɂ���Ԃ̍��E�ړ��Ǘ��j
public class Player_MoveState : PlayerGroundedState
{
    private float stepTimer = 0f;
    private float stepInterval = 0.35f;

    public Player_MoveState(Entity_Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

        // Switch to idle if not moving or hitting wall
        if (player.moveInput.x == 0 || player.isWallDetected)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Move the player
        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);

        // Play footsteps
        stepTimer += Time.deltaTime;
        if (stepTimer >= stepInterval)
        {
            SoundManager.instance.PlaySFX("footstep", player.GetComponentInChildren<AudioSource>());
            stepTimer = 0f;
        }
    }
}
