using UnityEngine;

public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int amountToPierce;

    public override void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        base.SetupSword(manager, direction);

        amountToPierce = manager.amountToPierce; // safe now, manager is set
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (amountToPierce <= 0 || groundHit) // Stop the sword if ground is hit first or can't pierce anymore
        {
            DamageEnemiesInRadius(transform, 0.3f);
            StopSword(collision);
            return;
        }

        amountToPierce--;
        DamageEnemiesInRadius(transform, 0.3f);
    }
}
