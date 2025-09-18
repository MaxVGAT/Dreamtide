using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable, ISaveable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();       // Rigidbody2D参照
    private Collider2D col => GetComponentInChildren<Collider2D>();         // コライダー参照
    private Animator anim => GetComponentInChildren<Animator>();            // Animator参照
    private Entity_VFX vfx => GetComponent<Entity_VFX>();                   // VFX参照

    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>(); // ドロップ管理参照

    [SerializeField] private bool canDropItems = true;  // アイテムドロップ可能フラグ
    [SerializeField] private string chestID;            // チェスト識別ID

    // ダメージ処理、アイテムドロップと開放アニメーション開始
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canDropItems == false)
            return false;

        canDropItems = false;
        dropManager?.DropItems();                             // アイテムドロップ
        vfx.HandleHitColor(Entity_VFX.FlashType.White);       // ヒットVFX
        anim.SetBool("openChest", true);                      // 開封アニメーション
        rb.linearVelocity = new Vector2(0, 3);               // 弾む初速度
        rb.angularVelocity = Random.Range(-200, 200);        // 回転付与

        ChangeRBToKinematic();                                // RigidbodyをKinematic化

        return true;
    }

    // Rigidbodyを固定して物理挙動を停止
    private void ChangeRBToKinematic()
    {
        col.enabled = false;               // コライダー無効化
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;  // 速度リセット
        rb.angularVelocity = 0f;           // 回転リセット
    }

    // 物理挙動リセット用コルーチン（読み込み時）
    private IEnumerator FixDeadChestPhysics()
    {
        yield return null;                 // 1フレーム待機
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // セーブデータにチェストIDを追加
    public void SaveData(ref GameData gameData)
    {
        if (!gameData.openedChests.Contains(chestID))
            gameData.openedChests.Add(chestID);
    }

    // ロード時、開封状態再現
    public void LoadData(GameData gameData)
    {
        if (gameData.openedChests.Contains(chestID))
        {
            canDropItems = false;
            anim.SetBool("openChest", true);
            StartCoroutine(FixDeadChestPhysics());  // 物理挙動リセット
        }
    }
}
