using System;
using System.Collections;
using UnityEngine;

public class Entity_Enemy : Entity
{

    private Entity_VFX entityVFX;


    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;
    public EnemyBlockState blockState;

    [Header("Battle details")]
    public float battleMoveSpeed = 4;
    public float attackDistance = 1;
    public float battleTimeDuration = 3;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Movement details")]
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Range(0, 2)] public float moveAnimSpeedMultiplier = 1f;

    [Header("Stun details")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(8, 4);
    [SerializeField] protected bool canBeStunned;

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform player { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        entityVFX = GetComponent<Entity_VFX>();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalBattleSpeed = battleMoveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        battleMoveSpeed = battleMoveSpeed * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        battleMoveSpeed = originalBattleSpeed;
        anim.speed = originalAnimSpeed;
    }

    public void EnableCounterAttack(bool enable) => canBeStunned = enable;

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
    public RaycastHit2D PlayerIsDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

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

    private void OnEnable()
    {
        Entity_Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeath -= HandlePlayerDeath;
    }

}
