using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    private SpriteRenderer sr;                // 表示用スプライト

    [SerializeField] private Item_DataSO itemData; // アイテムデータ

    private Inventory_Item itemToAdd;         // インベントリに追加するアイテム
    private Inventory_Base inventory;         // 接触したインベントリ参照

    private void Awake()
    {
        itemToAdd = new Inventory_Item(itemData); // アイテムインスタンス生成
    }

    private void OnValidate()
    {
        if (itemData == null) return;

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemData.itemIcon;   // アイコン設定
        gameObject.name = "Object_ItemPickup - " + itemData.itemName; // 名前更新
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inventory = collision.GetComponent<Inventory_Base>(); // インベントリ取得

        if (inventory == null) return;

        // アイテムを追加可能かチェック
        bool canAddItem = inventory.CanAddItem() || inventory.FindStackable(itemToAdd) != null;

        if (canAddItem)
        {
            inventory.AddItem(itemToAdd); // アイテム追加
            Destroy(gameObject);           // オブジェクト破棄
        }
    }
}
