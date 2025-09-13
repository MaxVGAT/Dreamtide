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

    // �X���b�g�N���b�N��
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // �X���b�g����A�܂��̓}�e���A���͏������Ȃ�
        if (itemInSlot == null || itemInSlot.itemData.itemType == Item_Type.Material)
            return;

        bool alternativeInput = Input.GetKey(KeyCode.LeftControl);

        if (alternativeInput)
        {
            inventory.RemoveOneItem(itemInSlot);
        }
        else
        {

            // �_�u���N���b�N����
            if (Time.time - lastClickTime < DoubleClickThreshold)
            {
                inventory.TryUseItem(itemInSlot);
                lastClickTime = 0;
            }
            else
            {
                lastClickTime = Time.time;
            }

            if (itemInSlot == null)
                ui.itemTooltip.ShowToolTip(false, null);
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
