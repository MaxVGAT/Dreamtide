using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// �v���C���[�̃C���x���g��UI��Ǘ�����N���X
public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory; // �v���C���[�̃C���x���g���f�[�^

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent; // �A�C�e���X���b�g�̐e�I�u�W�F�N�g
    [SerializeField] private UI_EquipSlotParent equipSlotParent; // �h��X���b�g�̐e

    // ������
    private void Awake()
    {
        // �v���C���[�C���x���g����������Ď擾
        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI; // �C���x���g���X�V����UI�X�V

        UpdateUI(); // �ŏ���UI��X�V
    }

    // �C���x���g���S�̂�UI�X�V
    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);  // �A�C�e���X���b�g�X�V
        equipSlotParent.UpdateEquipmentSlots(inventory.equipList);
    }

    // �C���x���g��UI������I�ɍX�V�i���C�A�E�g������č\�z�j
    public void RefreshInventoryUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
