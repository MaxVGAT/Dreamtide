using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Collider2D col => GetComponentInChildren<Collider2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();


    public void TakeDamage(float damage, Transform damageDealer)
    {
        vfx.HandleHitColor(Entity_VFX.FlashType.White);
        anim.SetBool("openChest", true);
        rb.linearVelocity = new Vector2(0, 3);

        rb.angularVelocity = Random.Range(-200, 200);

        ChangeRBToKinematic();
    }

    private void ChangeRBToKinematic()
    {
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

}