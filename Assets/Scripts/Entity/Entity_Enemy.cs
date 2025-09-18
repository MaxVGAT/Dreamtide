using System;
using System.Collections;
using UnityEngine;

// 敵エンティティ管理
public class Entity_Enemy : Entity
{
    private Entity_VFX entityVFX; // VFX参照
    public Enemy_Health health { get; private set; } // HP管理
    public Entity_Stats stats { get; private set; } // ステータス参照

    // 敵ステート
    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;

    [Header("Battle details")] // 戦闘関連設定
    public float battleMoveSpeed = 4;
    public float attackDistance = 1;
    public float battleTimeDuration = 3;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Exp Details")] // 経験値報酬
    [SerializeField] private int experienceReward = 10;
    private Entity_Player player;

    [Header("Movement details")] // 移動関連
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Range(0, 2)] public float moveAnimSpeedMultiplier = 1f;

    [Header("Stun details")] // スタン関連
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(8, 4);
    [SerializeField] protected bool canBeStunned;

    [Header("Player detection")] // プレイヤー検知関連
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform playerTransform { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1f;

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier; // 実際の移動速度取得
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier; // 戦闘時速度取得

    protected override void Awake()
    {
        base.Awake();
        entityVFX = GetComponent<Entity_VFX>();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
    }

    protected override void Start()
    {
        player = FindAnyObjectByType<Entity_Player>(); // プレイヤー参照取得
    }

    // 移動速度減少処理
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1 - slowMultiplier;
        anim.speed *= activeSlowMultiplier;
        yield return new WaitForSeconds(duration);
        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1;
        anim.speed = 1;
        base.StopSlowDown();
    }

    public void EnableCounterAttack(bool enable) => canBeStunned = enable;

    // 死亡処理
    public override void EntityDeath()
    {
        base.EntityDeath();

        if (entityVFX != null) entityVFX.StopAllVfx(); // VFX停止

        var uiInGame = FindFirstObjectByType<UI_InGame>();
        if (uiInGame != null) uiInGame.AddExperience(experienceReward); // 経験値付与

        stateMachine.ChangeState(deadState); // 死亡ステートへ
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState); // プレイヤー死亡時は待機へ
    }

    // 戦闘ステート遷移
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState) return;

        this.playerTransform = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (playerTransform == null) playerTransform = PlayerIsDetected().transform;
        return playerTransform;
    }

    // プレイヤー検知判定
    public RaycastHit2D PlayerIsDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player")) return default;
        return hit;
    }

    // ギズモ描画（範囲可視化）
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow; // プレイヤー検知距離
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue; // 攻撃距離
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green; // 最小退避距離
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * minRetreatDistance), playerCheck.position.y));
    }

    private void OnEnable()
    {
        Entity_Player.OnPlayerDeath += HandlePlayerDeath; // プレイヤー死亡イベント購読
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeath -= HandlePlayerDeath; // イベント解除
    }
}
