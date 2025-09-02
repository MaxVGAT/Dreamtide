using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine; // �X�e�[�g�}�V���ւ̎Q��
    protected string animBoolName;       // ���̃X�e�[�g�ɑΉ�����A�j���[�V������Bool��

    protected Animator anim;             // �A�j���[�^�[�ւ̎Q��
    protected Rigidbody2D rb;            // Rigidbody2D�ւ̎Q��
    protected Entity_Stats stats;        // �G���e�B�e�B�̃X�e�[�^�X���

    protected float stateTimer;          // �X�e�[�g��ł̌o�ߎ��ԊǗ�
    public bool triggerCalled;        // �A�j���[�V�����C�x���g�̔��ΊǗ��t���O

    // �R���X�g���N�^�F�X�e�[�g�}�V���ƃA�j���[�V�������������
    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    // �X�e�[�g�ɓ������Ƃ��ɌĂ΂��
    // �A�j���[�V������Bool��true�ɂ��A�g���K�[�t���O����Z�b�g
    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    // ���t���[���X�V
    // �^�C�}�[����炵�A�A�j���[�V�����p�����[�^��X�V
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
    }

    // �X�e�[�g�𔲂���Ƃ��ɌĂ΂��
    // �A�j���[�V������Bool��false�ɂ���
    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
    }

    // �A�j���[�V�����C�x���g����Ă΂��
    // �g���K�[�t���O��true�ɐݒ�
    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    // �X�e�[�g�ŗL�̃A�j���[�V�����p�����[�^�X�V����
    public virtual void UpdateAnimationParameters()
    {

    }

    // �U�����x��A�j���[�^�[�ɓ���
    public void SyncAttackSpeed()
    {
        float attackSpeed = stats.offense.attackSpeed.GetValue();
        anim.SetFloat("attackSpeedMultiplier", attackSpeed);
    }
}
