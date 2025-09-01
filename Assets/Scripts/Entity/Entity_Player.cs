using System;
using System.Collections;
using System.Xml;
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

    #region State Variables
    // プレイヤーの各状態
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
    private Coroutine queuedAttackCo; // 攻撃入力遅延用コルーチン

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

        // 必要なコンポーネントを取得
        input = new PlayerInputSet();
        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();

        // 各状態を初期化
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
        // 初期状態をIdleに設定
        stateMachine.Initialize(idleState);
    }

    // プレイヤーを瞬間移動
    public void TeleportPlayer(Vector3 position) => transform.position = position;

    // ガード状態かどうか
    public override bool isBlocking => stateMachine.currentState is Player_BlockState;

    // スローダウン処理（スキルや状態異常で速度低下）
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        // 元の値を保存
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpDir;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        // 速度を低下
        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        wallJumpDir *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier; // 全攻撃速度も低下
        }

        yield return new WaitForSeconds(duration);

        // 元の値に戻す
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

    // 死亡処理
    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke(); // 他クラスに死亡通知
        stateMachine.ChangeState(deadState); // 死亡状態に移行
    }

    // 攻撃入力遅延処理
    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCO());
    }

    private IEnumerator EnterAttackStateWithDelayCO()
    {
        yield return new WaitForEndOfFrame(); // フレームが終わるまで待つ
        stateMachine.ChangeState(basicAttackState); // 攻撃状態に移行
    }

    private void OnEnable()
    {
        input.Enable();

        // マウス座標取得
        input.Player.Mouse.performed += context => mousePosition = context.ReadValue<Vector2>();

        // 移動入力
        input.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        input.Player.Movement.canceled += context => moveInput = Vector2.zero;

        // UI切り替え
        input.Player.ToggleUI.performed += context => ui.ToggleUI();

        // スキル使用
        input.Player.Skill.performed += context => skillManager.shard.TryUseSkill();
        input.Player.Skill.performed += context => skillManager.timeEcho.TryUseSkill();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
