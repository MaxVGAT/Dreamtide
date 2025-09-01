using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVfx;      // 死亡時のエフェクト
    [SerializeField] private LayerMask whatIsGround;     // 地面判定レイヤー
    [SerializeField] private float wispMoveSpeed = 15;   // ウィスプ移動速度
    private bool shouldMoveToPlayer;                     // プレイヤーに戻るかフラグ

    private Transform playerTransform;
    private Player_SkillManager skillManager;
    private Entity_Health playerHealth;
    private SkillObject_Health echoHealth;
    private Entity_StatusHandler statusHandler;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;

    public int maxAttacks { get; private set; }          // 最大攻撃回数

    // 初期設定
    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        playerStats = echoManager.player.stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();
        playerTransform = echoManager.transform.root;
        playerHealth = echoManager.player.health;
        skillManager = echoManager.skillManager;

        // エコーの寿命後に死亡処理
        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
        FlipToTarget();

        echoHealth = GetComponent<SkillObject_Health>();
        wispTrail = GetComponentInChildren<TrailRenderer>();
        wispTrail.gameObject.SetActive(false);

        anim.SetBool("canAttack", maxAttacks > 0);
    }

    // 攻撃処理
    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1); // 範囲攻撃
        if (!targetGotHit)
            return;

        // 一定確率で時間エコーを複製
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 2 : -2;
        if (canDuplicate)
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0));
    }

    private void Update()
    {
        if (shouldMoveToPlayer)
            HandleWispMovement(); // ウィスプ移動
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
            StopHorizontalMovement(); // 横方向移動を停止して落下処理
        }
    }

    // プレイヤーへ移動する処理
    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);

        // プレイヤー到達時に回復やクールダウン短縮を行う
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.05f)
        {
            HandlePlayerTouch();
            Destroy(gameObject);
        }
    }

    // プレイヤーに触れたときの効果
    private void HandlePlayerTouch()
    {
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();
        playerHealth.IncreaseHealth(healAmount);

        float amountInSeconds = echoManager.GetCooldownReduceInSeconds();
        skillManager.ReduceAllSkillsBooldownBy(amountInSeconds);

        if (echoManager.CanRemoveNegativeEffects())
            statusHandler.RemoveAllNegativeEffects();
    }

    // 敵に向かって向きを変える
    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();

        if (target != null && target.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);
    }

    // 死亡処理
    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);

        if (echoManager.ShouldBeWisp())
            TurnIntoWisp(); // ウィスプ化
        else
            Destroy(gameObject);
    }

    // ウィスプ化処理
    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false);
        wispTrail.gameObject.SetActive(true);
        rb.simulated = false; // 物理挙動停止
    }

    // 地面接触時に横方向速度を停止
    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.45f, whatIsGround);
        if (hit.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
