using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Collider2D col => GetComponentInChildren<Collider2D>(); 
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();

    // ダメージを受けた時の処理
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        vfx.HandleHitColor(Entity_VFX.FlashType.White); // ヒットVFX表示
        anim.SetBool("openChest", true);               // 開くアニメーション
        rb.linearVelocity = new Vector2(0, 3);         // 上方向の反動

        rb.angularVelocity = Random.Range(-200, 200);  // 回転付与

        ChangeRBToKinematic();                         // 物理停止

        return true;
    }

    // RigidbodyをKinematicに変更して動きを止める
    private void ChangeRBToKinematic()
    {
        col.enabled = false;               // 当たり判定無効化
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;  // 速度リセット
        rb.angularVelocity = 0f;           // 回転リセット
    }
}
