using System;
using System.Collections;
using UnityEngine;

public class Entity_Player : Entity
{
    public static Entity_Player instance; // プレイヤーのシングルトン参照

    public static event Action OnPlayerDeath; // 死亡時イベント
    public static event Action OnPlayerDeathFinished; // 死亡アニメ完了後イベント

    public UI ui { get; private set; } // UI参照
    public PlayerInputSet input { get; private set; } // 入力管理
    public Player_SkillManager skillManager { get; private set; } // スキル管理
    public Player_VFX vfx { get; private set; } // VFX管理
    public Entity_Health health { get; private set; } // HP管理
    public Entity_StatusHandler statusHandler { get; private set; } // ステータス管理
    public Player_Combat combat { get; private set; } // 戦闘関連
    public Inventory_Player inventory { get; private set; } // インベントリ管理
    public Player_Stats stats { get; private set; } // ステータス値

    #region State Variables
    // プレイヤー状態管理
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
    public Vector2[] attackVelocity; // 攻撃のベロシティ配列
    public Vector2 jumpAttackVelocity; // 空中攻撃のベロシティ
    public float attackVelocityDuration = 0.1f; // 攻撃速度持続時間
    public float comboAttackWindow = 1f; // コンボ入力受付時間
    private Coroutine queuedAttackCo; // 攻撃遅延用コルーチン

    [Header("Domain Ability Details")]
    public float riseSpeed = 25; // ドメイン能力上昇速度
    public float riseMaxDistance; // 最大上昇距離

    [Header("Movement details")]
    public float moveSpeed; // 移動速度
    public float jumpForce = 5f; // ジャンプ力
    public Vector2 wallJumpDir; // 壁ジャンプ方向
    public float inAirMoveMultiplier = 0.8f; // 空中移動補正
    public float wallSlideSlowMultiplier = 0.7f; // 壁滑り減速倍率
    [Space] public float dashDuration = 0.25f; // ダッシュ時間
    public float dashSpeed = 20f; // ダッシュ速度
    public Vector2 mousePosition { get; private set; } // マウス座標
    public Vector2 moveInput { get; private set; } // 移動入力

    protected override void Awake()
    {
        base.Awake();

        instance = this; // シングルトン設定

        // コンポーネント取得
        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        combat = GetComponent<Player_Combat>();
        inventory = GetComponent<Inventory_Player>();
        stats = GetComponent<Player_Stats>();

        input = new PlayerInputSet();
        ui.SetupControlsUI(input);

        // 状態インスタンス初期化
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

        stateMachine.Initialize(idleState); // 初期状態設定
    }

    protected override void Start()
    {
        base.Start();
        // Idle状態設定（必要なら追加処理）
    }

    // プレイヤー瞬間移動
    public void TeleportPlayer(Vector3 position) => transform.position = position;

    // ブロック状態判定
    public override bool isBlocking => stateMachine.currentState is Player_BlockState;

    // 移動速度・攻撃力遅延コルーチン
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpDir;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        // 速度調整
        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        wallJumpDir *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier; // 攻撃速度調整
        }

        yield return new WaitForSeconds(duration);

        // 元に戻す
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

    public void NotifyDeathFinished()
    {
        OnPlayerDeathFinished?.Invoke(); // 死亡アニメ完了通知
    }

    // 死亡処理
    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke(); // 死亡イベント呼び出し
        stateMachine.ChangeState(deadState); // 状態変更
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
        yield return new WaitForEndOfFrame(); // フレーム待機
        stateMachine.ChangeState(basicAttackState); // 攻撃状態へ
    }

    // 近くのオブジェクトと相互作用
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

        closest.GetComponent<IInteractable>().Interact(); // インタラクト実行
    }

    private void OnEnable()
    {
        input.Enable();

        // マウス位置取得
        input.Player.Mouse.performed += context => mousePosition = context.ReadValue<Vector2>();

        // 移動入力
        input.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        input.Player.Movement.canceled += context => moveInput = Vector2.zero;

        // インタラクト
        input.Player.Interact.performed += context => TryInteract();

        // クイックスロット使用
        input.Player.QuickItem_Slot1.performed += context => inventory.TryUseQuickItemInSlot(1);
        input.Player.QuickItem_Slot2.performed += context => inventory.TryUseQuickItemInSlot(2);

        // スキル使用
        input.Player.Skill.performed += context => skillManager.shard.TryUseSkill();
        input.Player.Skill.performed += context => skillManager.timeEcho.TryUseSkill();
    }

    private void OnDisable()
    {
        input.Disable(); // 入力無効化
    }
}
