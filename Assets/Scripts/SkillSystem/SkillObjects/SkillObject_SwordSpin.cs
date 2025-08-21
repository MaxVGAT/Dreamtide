using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;
    private float attacksPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        maxDistance = swordManager.maxDistance;
        attacksPerSecond = swordManager.attacksPerSecond;

        Invoke(nameof(GetSwordBackToPlayer), swordManager.maxSpinDuration);
    }

    protected override void Update()
    {
        transform.right = rb.linearVelocity;
        HandleAttack();
        HandleStopping();
        HandleComeback();
    }


    private void HandleStopping()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if(distanceToPlayer > maxDistance && rb.simulated == true)
        {
            rb.simulated = false;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Spin"))
                anim?.SetTrigger("spin");
        }
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if(attackTimer < 0 )
        {
            DamageEnemiesInRadius(transform, 0.7f);
            attackTimer = 1 / attacksPerSecond;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }
}
