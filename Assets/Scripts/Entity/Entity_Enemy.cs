using System;
using System.Collections;
using UnityEngine;

// 敵エンティティ（移動、戦闘、スタン、死亡処理などを管理）
public class Entity_Enemy : Entity
{

    private Entity_VFX entityVFX;
    public Enemy_Health health { get; private set; }

    // 敵の状態参照
    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;

    [Header("Battle details")] // 戦闘時の移動・攻撃・退避設定
    public float battleMoveSpeed = 4;
    public float attackDistance = 1;
    public float battleTimeDuration = 3;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Movement details")] // 通常移動や待機の設定
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Range(0, 2)] public float moveAnimSpeedMultiplier = 1f;

    [Header("Stun details")] // スタンの継続時間・吹き飛び・スタン可否
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(8, 4);
    [SerializeField] protected bool canBeStunned;

    [Header("Player detection")] // プレイヤー検知のためのレイキャスト設定
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1f;

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    protected override void Awake()
    {
        base.Awake();
        entityVFX = GetComponent<Entity_VFX>();
        health = GetComponent<Enemy_Health>();
    }

    // 一時的に移動速度とアニメ速度を低下させる
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1 - slowMultiplier;

        anim.speed = anim.speed * activeSlowMultiplier;

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

    // 敵の死亡処理（VFX停止と死亡ステート遷移）
    public override void EntityDeath()
    {
        base.EntityDeath();

        if (entityVFX != null)
            entityVFX.StopAllVfx();

        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    // 戦闘状態に入れるか確認して遷移
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerIsDetected().transform;

        return player;
    }

    // プレイヤー検知のレイキャスト
    public RaycastHit2D PlayerIsDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    // エディタ上で検知・攻撃・退避範囲を可視化
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * minRetreatDistance), playerCheck.position.y));
    }

    // プレイヤー死亡イベントの購読開始
    private void OnEnable()
    {
        Entity_Player.OnPlayerDeath += HandlePlayerDeath;
    }

    // プレイヤー死亡イベントの購読解除
    private void OnDisable()
    {
        Entity_Player.OnPlayerDeath -= HandlePlayerDeath;
    }

}
