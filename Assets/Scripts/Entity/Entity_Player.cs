using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class Entity_Player : Entity
{
    public static event Action OnPlayerDeath;

    public PlayerInputSet input { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerBasicAttackState basicAttackState { get; private set; }
    public playerJumpAttackState jumpAttackState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerBlockState blockState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboAttackWindow = 1f;
    private Coroutine queuedAttackCo;

    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5f;
    public Vector2 wallJumpDir;
    public float inAirMoveMultiplier = 0.8f;
    public float wallSlideSlowMultiplier = 0.7f;
    [Space] public float dashDuration = 0.25f;
    public float dashSpeed = 20f;

    public Vector2 moveInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        input = new PlayerInputSet();

        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new playerJumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new PlayerDeadState(this, stateMachine, "death");
        blockState = new PlayerBlockState(this, stateMachine, "block");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "counterAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public override bool isBlocking => stateMachine.currentState is PlayerBlockState;

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpDir;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        jumpForce = jumpForce * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;
        wallJumpDir = wallJumpDir * speedMultiplier;
        jumpAttackVelocity = jumpAttackVelocity * speedMultiplier;

        for(int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpDir = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;

        for(int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }   

    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCO());
    }

    private IEnumerator EnterAttackStateWithDelayCO()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        input.Player.Movement.canceled += context => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
