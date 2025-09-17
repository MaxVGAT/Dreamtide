using UnityEngine;

// �v���C���[�̃J�E���^�[�U�����
public class Player_CounterAttackState : PlayerState
{
    private Entity_VFX vfx;          // VFX�Ǘ��p
    private Player_Combat combat;     // �v���C���[�̐퓬�N���X
    private bool counteredSomething;  // �J�E���^�[������������

    public Player_CounterAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
        vfx = player.GetComponent<Entity_VFX>();
    }

    public override void Enter()
    {
        base.Enter();

        // �J�E���^�[��̉񕜎��Ԃ�ݒ�
        stateTimer = combat.GetCounterRecovery();



        // �J�E���^�[�U������s
        bool isCrit;
        counteredSomething = combat.CounterAttackPerformed(out isCrit);

        anim.SetBool("counterAttackPerformed", counteredSomething);

        // �J�E���^�[�����������ꍇ�AVFX�𐶐�
        if (counteredSomething && combat.counteredTargetTransform != null)
        {
            player.stats.GetElementalDamage(out ElementType element);
            vfx.CreateOnHitVFX(combat.counteredTargetTransform, isCrit, element);
            SoundManager.instance.PlaySFX("block/counter", player.GetComponentInChildren<AudioSource>());
        }
    }

    public override void Update()
    {
        base.Update();

        // �U�����̓v���C���[���~
        player.SetVelocity(0, rb.linearVelocity.y);

        // �A�j���[�V�����̃g���K�[�ŏ�Ԃ�I��
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        // �J�E���^�[�Ɏ��s�����ꍇ�A�^�C�}�[�ŏI��
        if (stateTimer < 0 && counteredSomething == false)
            stateMachine.ChangeState(player.idleState);
    }
}
