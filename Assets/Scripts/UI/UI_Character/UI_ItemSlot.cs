using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// �C���x���g����1�X���b�gUI��Ǘ�����N���X
// �}�e���A���ȊO�̃A�C�e���̓N���b�N�ő����\
public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; } // ���̃X���b�g�ɓ����Ă���A�C�e��
    protected Inventory_Player inventory; // �v���C���[�̃C���x���g��
    protected UI ui; // UI�S�̂ւ̎Q��
    protected RectTransform rect; // �X���b�g��RectTransform

    [Header("UI Slot Setup")]
    [SerializeField] protected Image itemIcon; // �A�C�e���A�C�R��
    [SerializeField] protected TextMeshProUGUI itemStackSize; // �A�C�e���̃X�^�b�N���\��

    // �_�u���N���b�N�p
    protected float lastClickTime;
    protected const float DoubleClickThreshold = 0.2f; // �_�u���N���b�N�ƔF������b��

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (ui == null) ui = FindAnyObjectByType<UI>();
        if (inventory == null) inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public virtual void Setup(UI ui, Inventory_Player inventory)
    {
        this.ui = ui;
        this.inventory = inventory;
        this.rect = GetComponent<RectTransform>();
    }

    protected void OnEnable()
    {
        if (inventory == null)
            inventory = FindAnyObjectByType<Inventory_Player>();
    }

    protected void Update()
    {
        if (inventory == null)
        {
            Debug.LogWarning($"[UI_ItemSlot] Inventory is NULL in scene '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}' for slot '{name}'");
        }
        else
        {
            // Optional: show which item is in slot
            string itemName = itemInSlot != null ? itemInSlot.itemData.itemName : "None";
            Debug.Log($"[UI_ItemSlot] Inventory OK. Slot '{name}' has item: {itemName}");
        }
    }


    // �X���b�g�N���b�N��
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (inventory == null) inventory = FindAnyObjectByType<Inventory_Player>();
        if (inventory == null || itemInSlot == null || itemInSlot.itemData == null) return;

        // Ctrl for removing 1 stack
        if (Input.GetKey(KeyCode.LeftControl))
        {
            inventory.RemoveOneItem(itemInSlot);
            return;
        }

        // If equipment, single click unequips
        if (itemInSlot.itemData.itemType != Item_Type.Consumables)
        {
            inventory.UnequipItem(itemInSlot);
            return;
        }

        // Otherwise, consumables use double-click
        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.TryUseItem(itemInSlot);
            lastClickTime = 0;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }




    // �X���b�g�̓�e��X�V
    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            // �X���b�g����Ȃ�A�C�R���ƃX�^�b�N�����\��
            if (itemStackSize != null) itemStackSize.text = " ";
            if (itemIcon != null) itemIcon.color = Color.clear;
            return;
        }

        // �A�C�R����X�V
        if (itemIcon != null)
        {
            Color color = Color.white;
            color.a = 0.9f; // ��������
            itemIcon.color = color;
            itemIcon.sprite = itemInSlot.itemData.itemIcon;
        }

        // �X�^�b�N����X�V
        if (itemStackSize != null)
        {
            itemStackSize.text = item.stackSize > 1 ? item.stackSize.ToString() : " ";
            itemStackSize.color = item.stackSize < itemInSlot.itemData.maxStackSize ? Color.white : Color.yellow;
        }
    }



    // �}�E�X�I�[�o�[�Ńc�[���`�b�v�\��
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        if (ui == null || ui.itemTooltip == null)
            return;

        bool isMerchantOpen = ui.IsMerchantVisible();
        bool isStorageOpen = ui.IsStorageVisible();

        if (isMerchantOpen)
            ui.itemTooltip.ShowToolTip(true, rect, itemInSlot, true, true, false, false);
        else if (isStorageOpen)
            ui.itemTooltip.ShowToolTip(true, rect, itemInSlot, true, false, false, true);
        else
            ui.itemTooltip.ShowToolTip(true, rect, itemInSlot, true, false, true, false);
    }

    // �}�E�X�����ꂽ��c�[���`�b�v��\��
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui == null || ui.itemTooltip == null)
            return;

        ui.itemTooltip.ShowToolTip(false, null);
    }
}
