using UnityEngine;

public class Object_ItemPickup : MonoBehaviour
{
    private SpriteRenderer sr;                // �\���p�X�v���C�g

    [SerializeField] private Item_DataSO itemData; // �A�C�e���f�[�^

    private Inventory_Item itemToAdd;         // �C���x���g���ɒǉ�����A�C�e��
    private Inventory_Base inventory;         // �ڐG�����C���x���g���Q��

    private void Awake()
    {
        itemToAdd = new Inventory_Item(itemData); // �A�C�e���C���X�^���X����
    }

    private void OnValidate()
    {
        if (itemData == null) return;

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemData.itemIcon;   // �A�C�R���ݒ�
        gameObject.name = "Object_ItemPickup - " + itemData.itemName; // ���O�X�V
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inventory = collision.GetComponent<Inventory_Base>(); // �C���x���g���擾

        if (inventory == null)
            return;


        if (inventory.CanAddItem(itemToAdd))
        {
            inventory.AddItem(itemToAdd); // �A�C�e���ǉ�
            Destroy(gameObject);           // �I�u�W�F�N�g�j��
        }
    }
}
