using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBasicAttackState : PlayerState
{

    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private bool comboAttackQueued;
    private const int FirstComboIndex = 1;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private int attackDirection;

    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
            comboLimit = player.attackVelocity.Length;
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded();

        attackDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();
        HandleAttackSliding();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
        {
            HandleStateExit();
        }
    }

    private void HandleAttackSliding()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        comboIndex++;
        lastTimeAttacked = Time.time;
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    private void ApplyAttackVelocity()
    {

        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDirection, attackVelocity.y);
    }
    
    private void ResetComboIndexIfNeeded()
    {
        if (comboIndex > comboLimit || Time.time > lastTimeAttacked + player.comboAttackWindow)
            comboIndex = FirstComboIndex;
    }
}
