using UnityEngine;
using UnityEngine.UIElements;

// �v���C���[��{�U�����
public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer; // �U���ɂ��ړ����Ԃ̃J�E���g
    private float lastTimeAttacked;    // �Ō�ɍU����������

    private bool comboAttackQueued;    // ���̃R���{�U�����\�񂳂ꂽ��
    private const int FirstComboIndex = 1;
    private int comboIndex = 1;        // ���݂̃R���{�ԍ�
    private int comboLimit = 3;        // �ő�R���{��
    private int attackDirection;       // �U������

    public Player_BasicAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
            comboLimit = player.attackVelocity.Length; // �R���{����U�����x�z��ɍ��킹��
    }

    public static class AnimatorDebugHelper
    {
        /// <summary>
        /// Logs useful animator and state info to help debug lock-ups.
        /// Call this inside Update() of a state or entity.
        /// </summary>
        public static void DebugAnimator(Animator anim, string stateName, bool triggerCalled, string animBoolName)
        {
            if (anim == null) return;

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            Debug.Log(
                $"[AnimatorDebug] " +
                $"State: {info.shortNameHash} (expected: {stateName}), " +
                $"NormalizedTime: {info.normalizedTime:F2}, " +
                $"IsName({stateName}): {info.IsName(stateName)}, " +
                $"{animBoolName}: {anim.GetBool(animBoolName)}, " +
                $"triggerCalled: {triggerCalled}"
            );
        }

    }
    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded(); // �R���{���Z�b�g����
        SyncAttackSpeed();          // �U�����x�A�j������

        attackDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity(); // �U�����̃X���C�h
    }

    public override void Update()
    {
        base.Update();
        HandleAttackSliding(); // �U�����̈ړ�����

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack(); // �R���{�\��

        if (triggerCalled)
            HandleStateExit(); // �U���I����̏�ԑJ��

        
    }

    // �U���ɂ��ړ��̏���
    private void HandleAttackSliding()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;               // �R���{�ԍ��i�s
        lastTimeAttacked = Time.time;
    }

    // �U����ԏI�����̏���
    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay(); // ���̃R���{��
        }
        else
            stateMachine.ChangeState(player.idleState); // �A�C�h����
    }

    // ���̍U����\��
    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    // �U�����Ƀv���C���[��O�i������
    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDirection, attackVelocity.y);
    }

    // ��莞�Ԍo�߂ŃR���{���Z�b�g
    private void ResetComboIndexIfNeeded()
    {
        if (comboIndex > comboLimit || Time.time > lastTimeAttacked + player.comboAttackWindow)
            comboIndex = FirstComboIndex;
    }
}
