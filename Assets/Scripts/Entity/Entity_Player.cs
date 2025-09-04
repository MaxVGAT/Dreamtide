using System;
using System.Collections;
using UnityEngine;

public class Entity_Player : Entity
{
    public static event Action OnPlayerDeath;

    private UI ui;
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }

    #region State Variables
    // �v���C���[�̊e���
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_BlockState blockState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }
    public Player_SwordThrowState swordThrowState { get; private set; }
    public Player_DomainState domainState { get; private set; }
    #endregion

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboAttackWindow = 1f;
    private Coroutine queuedAttackCo; // �U�����͒x���p�R���[�`��

    [Header("Domain Ability Details")]
    public float riseSpeed = 25;
    public float riseMaxDistance;

    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5f;
    public Vector2 wallJumpDir;
    public float inAirMoveMultiplier = 0.8f;
    public float wallSlideSlowMultiplier = 0.7f;
    [Space] public float dashDuration = 0.25f;
    public float dashSpeed = 20f;
    public Vector2 mousePosition { get; private set; }
    public Vector2 moveInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // �K�v�ȃR���|�[�l���g��擾
        input = new PlayerInputSet();
        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        combat = GetComponent<Player_Combat>();

        // �e��Ԃ������
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "death");
        blockState = new Player_BlockState(this, stateMachine, "block");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
        swordThrowState = new Player_SwordThrowState(this, stateMachine, "swordThrow");
        domainState = new Player_DomainState(this, stateMachine, "jumpFall");
    }

    protected override void Start()
    {
        base.Start();
        // ������Ԃ�Idle�ɐݒ�
        stateMachine.Initialize(idleState);
    }

    // �v���C���[��u�Ԉړ�
    public void TeleportPlayer(Vector3 position) => transform.position = position;

    // �K�[�h��Ԃ��ǂ���
    public override bool isBlocking => stateMachine.currentState is Player_BlockState;

    // �X���[�_�E�������i�X�L�����Ԉُ�ő��x�ቺ�j
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        // ���̒l��ۑ�
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpDir;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        // ���x��ቺ
        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        wallJumpDir *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier; // �S�U�����x��ቺ
        }

        yield return new WaitForSeconds(duration);

        // ���̒l�ɖ߂�
        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpDir = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    // ���S����
    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke(); // ���N���X�Ɏ��S�ʒm
        stateMachine.ChangeState(deadState); // ���S��ԂɈڍs
    }

    // �U�����͒x������
    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCO());
    }

    private IEnumerator EnterAttackStateWithDelayCO()
    {
        yield return new WaitForEndOfFrame(); // �t���[�����I���܂ő҂�
        stateMachine.ChangeState(basicAttackState); // �U����ԂɈڍs
    }

    private void TryInteract()
    {
        Transform closest = null;

        float closestDistance = Mathf.Infinity;

        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;
            }
        }

        if (closest == null)
            return;

        closest.GetComponent<IInteractable>().Interact();
    }

    private void OnEnable()
    {
        input.Enable();

        // �}�E�X���W�擾
        input.Player.Mouse.performed += context => mousePosition = context.ReadValue<Vector2>();

        // �ړ�����
        input.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        input.Player.Movement.canceled += context => moveInput = Vector2.zero;

        // UI�؂�ւ�
        input.Player.ToggleUI.performed += context => ui.ToggleUI();
        input.Player.Interact.performed += context => TryInteract();

        // �X�L���g�p
        input.Player.Skill.performed += context => skillManager.shard.TryUseSkill();
        input.Player.Skill.performed += context => skillManager.timeEcho.TryUseSkill();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
