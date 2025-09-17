using System;
using UnityEngine;

// �����X���b�g�̃f�[�^�Ǘ��N���X
[Serializable]
public class Inventory_EquipmentSlot
{
    public Item_Type slotType;       // ���̃X���b�g�̎�ށi��FHelmet, Ring�j
    public Inventory_Item equippedItem; // �������̃A�C�e��

    // �w��^�C�v�ŃX���b�g��쐬
    public Inventory_EquipmentSlot(Item_Type type)
    {
        slotType = type;
        equippedItem = null;
    }

    // �f�t�H���g�R���X�g���N�^�i�����Ȃ��j
    public Inventory_EquipmentSlot()
    {
        equippedItem = null;
    }

    // �X���b�g�ɑ������̃A�C�e�������邩
    public bool HasItem() => equippedItem != null && equippedItem.itemData != null;
}
