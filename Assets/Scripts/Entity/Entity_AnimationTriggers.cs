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

    public void OnLeapEnd()
    {
        // For example, move the monster forward by a fixed distance
        float leapDistance = 2f;
        Vector2 direction = new Vector2(entityEnemy.facingDirection, 0);
        entityEnemy.transform.position += (Vector3)(direction * leapDistance);
    }
}
