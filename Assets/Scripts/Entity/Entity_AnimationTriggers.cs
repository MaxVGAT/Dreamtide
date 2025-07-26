using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Enemy entityEnemy;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityEnemy = GetComponentInParent<Entity_Enemy>();
    }

    private void CurrentStateTrigger()
    {
        entity.CallAnimationTrigger();
    }
}
