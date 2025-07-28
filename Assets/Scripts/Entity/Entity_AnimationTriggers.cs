using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Enemy entityEnemy;
    private Entity_CombatComponent entityCombat;
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityEnemy = GetComponentInParent<Entity_Enemy>();
        entityCombat = GetComponentInParent<Entity_CombatComponent>();
    }

    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }
}
