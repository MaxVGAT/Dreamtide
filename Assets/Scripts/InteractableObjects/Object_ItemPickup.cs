// アイテム取得オブジェクト
using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    [SerializeField] private Item_DataSO itemData; // アイテムデータ参照

    [SerializeField] private SpriteRenderer sr;   // 見た目用SpriteRenderer
    [SerializeField] private Rigidbody2D rb;      // 物理挙動用Rigidbody2D
    [SerializeField] private Collider2D col;      // 衝突判定用Collider2D

    [Header("Drop Details")]
    [SerializeField] private Vector2 dropForce = new Vector2(3, 10); // ドロップ時の初速

    private void OnValidate()
    {
        if (itemData == null) return;

        sr = GetComponent<SpriteRenderer>(); // SpriteRenderer取得
        SetupVisuals();                      // 見た目をセット
    }

    // アイテム設定
    public void SetupItem(Item_DataSO itemData)
    {
        this.itemData = itemData;
        SetupVisuals();

        // ランダムに飛ばす
        float xForce = Random.Range(-dropForce.x, dropForce.x);
        rb.linearVelocity = new Vector2(xForce, dropForce.y);
        col.isTrigger = false; // 落下中は物理判定

        // プレイヤーとの衝突無視
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(col, playerCollider, true);
            }
        }
    }

    // 見た目セット
    private void SetupVisuals()
    {
        sr.sprite = itemData.itemIcon;   // アイコン設定
        gameObject.name = "Object_ItemPickup - " + itemData.itemName; // オブジェクト名設定
    }

    // 地面に着地したとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
        {
            col.isTrigger = true;                      // プレイヤー取得用Triggerに切替
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // 動きを固定

            // プレイヤーとの衝突再有効化
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Collider2D playerCollider = player.GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    Physics2D.IgnoreCollision(col, playerCollider, false);
                }
            }
        }
    }

    // プレイヤーが触れたとき
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory_Player inventory = collision.GetComponent<Inventory_Player>();

        if (inventory == null)
            return;

        Inventory_Item itemToAdd = new Inventory_Item(itemData);
        Inventory_Storage storage = inventory.storage;

        if (inventory.CanAddItem(itemToAdd))
        {
            inventory.AddItem(itemToAdd); // インベントリに追加
            Destroy(gameObject);           // オブジェクト削除
        }
    }
}
