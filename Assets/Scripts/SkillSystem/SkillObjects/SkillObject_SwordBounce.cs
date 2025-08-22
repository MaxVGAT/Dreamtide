using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed;
    private int bounceCount;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        anim.SetTrigger("spin");
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleComeback();
        HandleBounce();
    }

    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1);
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBackToPlayer();
            }
        }
    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget();
        bounceCount--;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // Get enemies detected the skill's zone
    {
        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10);
            rb.simulated = false;
        }

        DamageEnemiesInRadius(transform, 1);

        if (enemyTargets.Length <= 1 || bounceCount == 0)
            GetSwordBackToPlayer();
        else
            BounceToNextTarget();
    }

    private List<Transform> GetAliveTargets() // Goes through enemyTargets (found by OnTriggerEnter) and return the ones that are non-null.
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }
    private List<Transform> GetValidTargets() // Return a validTarget only if it has never been picked before. If all alive targets have been marked as valid, then the list resets.
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && selectedBefore.Contains(enemy.transform) == false)
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)
            return validTargets;
        else
        {
            selectedBefore.Clear();
            return aliveTargets;
        }
    }

    private Transform GetNextTarget() // Picks a random target from the ValidTargets list, remembers with selectedBefore, then returns it for next attack.
    {
        List<Transform> validTarget = GetValidTargets();

        int randomIndex = Random.Range(0, validTarget.Count);

        Transform nextTarget = validTarget[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }


}
