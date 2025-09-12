using UnityEngine;
using UnityEngine.EventSystems;

// �����X���b�gUI��Ǘ�����N���X
// UI_ItemSlot ��p�����āA�����\�ȃA�C�e���^�C�v��w��ł���
public class UI_EquipSlot : UI_ItemSlot
{
    public Item_Type slotType; // ���̃X���b�g�ő����\�ȃA�C�e���^�C�v

    // �G�f�B�^��ŃX���b�g��������X�V����
    private void OnValidate()
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();
    }

    // �X���b�g���N���b�N���ꂽ���̏���
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) // �X���b�g�ɃA�C�e�����Ȃ���Ή�����Ȃ�
            return;

        // �_�u���N���b�N����
        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.UnequipItem(itemInSlot); // �A�C�e����O��
            lastClickTime = 0;                 // �N���b�N�^�C�}�[����Z�b�g
        }
        else
            lastClickTime = Time.time;         // �N���b�N���Ԃ�X�V
    }
}
