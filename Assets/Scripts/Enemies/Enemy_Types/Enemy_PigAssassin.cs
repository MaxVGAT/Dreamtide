using UnityEngine;

public class Enemy_PigAssassin : Entity_Enemy, ICounterable // �s�b�O�A�T�V���G�ŗL�̏ڍׂ�����N���X
{
    public bool CanBeCountered { get => canBeStunned; } // CanBeCountered �� canBeStunned �ɐݒ�\�ȃt�H���[�A�b�v��ԗp�̃t���O

    // Enemy_VFX��I�[�o�[���C�h���ēG�̃A�j���[�V������Ԃ�K�p
    protected override void Awake()
    {
        base.Awake();

        // �e��Ԃ���ꂼ��̃X�N���v�g�ƃA�j���[�V�����ŏ�����
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        battleState = new EnemyBattleState(this, stateMachine, "battle");
        deadState = new EnemyDeadState(this, stateMachine, "death");
        stunnedState = new EnemyStunnedState(this, stateMachine, "stunned");
    }

    // Entity�e�X�N���v�g����A�C�h����Ԃ������
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    // �J�E���^�[�U���̃^�C�~���O�Ńu���b�N���ꂽ�ꍇ�AstunnedState�ɏ�Ԃ�ύX
    public void HandleCounterAttack()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
