using System;
using System.Collections;
using UnityEngine;

// �G�G���e�B�e�B�i�ړ��A�퓬�A�X�^���A���S�����Ȃǂ�Ǘ��j
public class Entity_Enemy : Entity
{

    private Entity_VFX entityVFX;
    public Enemy_Health health { get; private set; }
    public Entity_Stats stats { get; private set; }

    // �G�̏�ԎQ��
    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;

    [Header("Battle details")] // �퓬���̈ړ��E�U���E�ޔ�ݒ�
    public float battleMoveSpeed = 4;
    public float attackDistance = 1;
    public float battleTimeDuration = 3;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Exp Details")]
    [SerializeField] private int experienceReward = 10;
    private Entity_Player player;

    [Header("Movement details")] // �ʏ�ړ���ҋ@�̐ݒ�
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Range(0, 2)] public float moveAnimSpeedMultiplier = 1f;

    [Header("Stun details")] // �X�^���̌p�����ԁE������сE�X�^����
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2(8, 4);
    [SerializeField] protected bool canBeStunned;

    [Header("Player detection")] // �v���C���[���m�̂��߂̃��C�L���X�g�ݒ�
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform playerTransform { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1f;

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    protected override void Awake()
    {
        base.Awake();
        entityVFX = GetComponent<Entity_VFX>();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
    }

    protected override void Start()
    {
        player = FindAnyObjectByType<Entity_Player>();
    }

    // �ꎞ�I�Ɉړ����x�ƃA�j�����x��ቺ������
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

    // �G�̎��S�����iVFX��~�Ǝ��S�X�e�[�g�J�ځj
    public override void EntityDeath()
    {
        base.EntityDeath();

        if (entityVFX != null)
            entityVFX.StopAllVfx();


        var uiInGame = FindFirstObjectByType<UI_InGame>();
        if (uiInGame != null)
            uiInGame.AddExperience(experienceReward);


        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    // �퓬��Ԃɓ���邩�m�F���đJ��
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.playerTransform = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (playerTransform == null)
            playerTransform = PlayerIsDetected().transform;

        return playerTransform;
    }

    // �v���C���[���m�̃��C�L���X�g
    public RaycastHit2D PlayerIsDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    // �G�f�B�^��Ō��m�E�U���E�ޔ�͈͂����
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

    // �v���C���[���S�C�x���g�̍w�ǊJ�n
    private void OnEnable()
    {
        Entity_Player.OnPlayerDeath += HandlePlayerDeath;
    }

    // �v���C���[���S�C�x���g�̍w�ǉ��
    private void OnDisable()
    {
        Entity_Player.OnPlayerDeath -= HandlePlayerDeath;
    }

}
