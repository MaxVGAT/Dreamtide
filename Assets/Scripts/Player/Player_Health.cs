using UnityEngine;

public class Player_Health : Entity_Health
{
    private Entity_Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Die();
    }

}
