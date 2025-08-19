using UnityEngine;

public class Player_AnimationTrigger : Entity_AnimationTriggers
{
    private Entity_Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Entity_Player>();
    }

    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();
}
