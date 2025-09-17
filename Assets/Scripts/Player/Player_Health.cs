using UnityEngine;

public class Player_Health : Entity_Health
{
    private Entity_Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>();
    }

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        bool wasDamaged = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        if (wasDamaged && !isDead)
        {
            // Play damage SFX here
            SoundManager.instance.PlaySFX("playerHit", GetComponentInChildren<AudioSource>());
        }

        return wasDamaged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Die();


    }

}
