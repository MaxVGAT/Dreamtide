using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    [SerializeField] private Item_DataSO itemData; // �A�C�e���f�[�^

    [SerializeField] private SpriteRenderer sr;                // �\���p�X�v���C�g
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    [Header("Drop Details")]
    [SerializeField] private Vector2 dropForce = new Vector2(3, 10);

    private void OnValidate()
    {
        if (itemData == null) return;

        sr = GetComponent<SpriteRenderer>();
        SetupVisuals();
    }

    public void SetupItem(Item_DataSO itemData)
    {
        this.itemData = itemData;
        SetupVisuals();

        float xForce = Random.Range(-dropForce.x, dropForce.x);
        rb.linearVelocity = new Vector2 (xForce, dropForce.y);
        col.isTrigger = false;

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


    private void SetupVisuals()
    {
        sr.sprite = itemData.itemIcon;   // �A�C�R���ݒ�
        gameObject.name = "Object_ItemPickup - " + itemData.itemName; // ���O�X�V
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
        {
            col.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // Re-enable collision with player for trigger pickup
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory_Player inventory = collision.GetComponent<Inventory_Player>();

        if (inventory == null)
            return;

        Inventory_Item itemToAdd = new Inventory_Item(itemData);
        Inventory_Storage storage = inventory.storage;

        if (inventory.CanAddItem(itemToAdd))
        {
            inventory.AddItem(itemToAdd); // �A�C�e���ǉ�
            Destroy(gameObject);           // �I�u�W�F�N�g�j��
        }
    }
}
