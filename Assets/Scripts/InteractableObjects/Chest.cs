using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();


    public void TakeDamage(float damage, Transform damageDealer)
    {
        vfx.PlayOnDamageVfx();
        anim.SetBool("openChest", true);
        rb.linearVelocity = new Vector2(0, 3);

        rb.angularVelocity = Random.Range(-200, 200);
    }

}