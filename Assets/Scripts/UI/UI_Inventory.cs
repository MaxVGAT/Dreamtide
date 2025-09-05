using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// �v���C���[�̃C���x���g��UI��Ǘ�����N���X
public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory; // �v���C���[�̃C���x���g���f�[�^
    private UI_EquipSlot[] uiWeaponTrinketSlots; // ����E�A�N�Z�T���X���b�gUI
    private UI_EquipSlot[] uiArmorSlots; // �h��X���b�gUI

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent; // �A�C�e���X���b�g�̐e�I�u�W�F�N�g
    [SerializeField] private Transform uiWeaponTrinketParent; // ����E�A�N�Z�T���X���b�g�̐e
    [SerializeField] private Transform uiArmorSlotParent; // �h��X���b�g�̐e

    // ������
    private void Awake()
    {
        // �e�X���b�gUI��e�I�u�W�F�N�g����擾
        uiWeaponTrinketSlots = uiWeaponTrinketParent.GetComponentsInChildren<UI_EquipSlot>();
        uiArmorSlots = uiArmorSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        // �v���C���[�C���x���g����������Ď擾
        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI; // �C���x���g���X�V����UI�X�V

        UpdateUI(); // �ŏ���UI��X�V
    }

    // �C���x���g���S�̂�UI�X�V
    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);  // �A�C�e���X���b�g�X�V
        UpdateEquipmentSlots();   // �����X���b�g�X�V
    }

    // �����X���b�gUI�̍X�V
    private void UpdateEquipmentSlots()
    {
        // ����E�A�N�Z�T���X���b�g
        for (int i = 0; i < uiWeaponTrinketSlots.Length; i++)
        {
            if (i < inventory.equipList.Count)
            {
                var slot = inventory.equipList[i];
                uiWeaponTrinketSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }

        // �h��X���b�g
        for (int i = 0; i < uiArmorSlots.Length; i++)
        {
            int equipListIndex = i + 4; // �h��X���b�g��equipList��4�Ԗڈȍ~
            if (equipListIndex < inventory.equipList.Count)
            {
                var slot = inventory.equipList[equipListIndex];
                uiArmorSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }
    }

    // �C���x���g��UI������I�ɍX�V�i���C�A�E�g������č\�z�j
    public void RefreshInventoryUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
