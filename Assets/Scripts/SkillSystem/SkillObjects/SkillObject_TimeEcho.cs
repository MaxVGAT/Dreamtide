using UnityEngine;
public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wispMoveSpeed = 15;
    private bool shouldMoveToPlayer;

    private Transform playerTransform;
    private Player_SkillManager skillManager;
    private Entity_Health playerHealth;
    private SkillObject_Health echoHealth;
    private Entity_StatusHandler statusHandler;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;


    public int maxAttacks { get; private set; }


    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        playerStats = echoManager.player.stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();
        playerTransform = echoManager.transform.root;
        playerHealth = echoManager.player.health;
        skillManager = echoManager.skillManager;

        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
        FlipToTarget();

        echoHealth = GetComponent<SkillObject_Health>();
        wispTrail = GetComponentInChildren<TrailRenderer>();
        wispTrail.gameObject.SetActive(false);

        anim.SetBool("canAttack", maxAttacks > 0);
    }

    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1);
        if (targetGotHit == false)
            return;
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 2 : -2;
        if (canDuplicate)
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0));
    }

    private void Update()
    {
        if (shouldMoveToPlayer)
            HandleWispMovement();
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
            StopHorizontalMovement();
        }
    }

    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, playerTransform.position) < 0.05f)
        {
            HandlePlayerTouch();
            Destroy(gameObject);
        }
    }

    private void HandlePlayerTouch()
    {
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();
        playerHealth.IncreaseHealth(healAmount);

        float amountInSeconds = echoManager.GetCooldownReduceInSeconds();
        skillManager.ReduceAllSkillsBooldownBy(amountInSeconds);

        if(echoManager.CanRemoveNegativeEffects())
            statusHandler.RemoveAllNegativeEffects();
    }

    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();

        if (target != null && target.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);

        if (echoManager.ShouldBeWisp())
            TurnIntoWisp();
        else
            Destroy(gameObject);
    }

    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false);
        wispTrail.gameObject.SetActive(true);
        rb.simulated = false;
    }

    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.45f, whatIsGround);
        if (hit.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}