using System.Collections;
using UnityEngine;

// �v���C���[�̎��S���
public class Player_DeadState : PlayerState
{
    [SerializeField] private float deathAnimDuration = 2f;

    public Player_DeadState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SoundManager.instance.PlaySFX("death", player.GetComponentInChildren<AudioSource>());
        // ���͂𖳌���
        input.Disable();

        // �����������~
        rb.simulated = false;

        player.StartCoroutine(WaitAndNotifyDeath());
    }

    private IEnumerator WaitAndNotifyDeath()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        // Notify listeners that player death finished
        player.NotifyDeathFinished();
    }
}
