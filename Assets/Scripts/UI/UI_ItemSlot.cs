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
    [SerializeField] private Image itemIcon; // �A�C�e���A�C�R��
    [SerializeField] private TextMeshProUGUI itemStackSize; // �A�C�e���̃X�^�b�N���\��

    // �_�u���N���b�N�p
    protected float lastClickTime;
    protected const float DoubleClickThreshold = 0.2f; // �_�u���N���b�N�ƔF������b��

    protected void Awake()
    {
        ui = GetComponentInParent<UI>(); // UI�R���|�[�l���g�擾
        inventory = FindAnyObjectByType<Inventory_Player>(); // �v���C���[�C���x���g���擾
        rect = GetComponent<RectTransform>(); // RectTransform�擾
    }

    // �X���b�g�N���b�N��
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // �X���b�g����A�܂��̓}�e���A���͏������Ȃ�
        if (itemInSlot == null || itemInSlot.itemData.itemType == Item_Type.Material)
            return;

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

        bool isMerchantOpen = ui.IsMerchantVisible();

        if (isMerchantOpen)
            ui.itemTooltip.ShowToolTip(true, rect, itemInSlot, true, true);
        else
            ui.itemTooltip.ShowToolTip(true, rect, itemInSlot);
    }

    // �}�E�X�����ꂽ��c�[���`�b�v��\��
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.ShowToolTip(false, null);
    }
}
