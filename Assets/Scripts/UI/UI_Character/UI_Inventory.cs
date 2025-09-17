using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �v���C���[�̃C���x���g��UI��Ǘ�����N���X
public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory; // �v���C���[�̃C���x���g���f�[�^

    [SerializeField] private UI_ItemSlotParent inventorySlotsParent; // �A�C�e���X���b�g�̐e�I�u�W�F�N�g
    [SerializeField] private UI_EquipSlotParent equipSlotParent; // �h��X���b�g�̐e
    [SerializeField] private TextMeshProUGUI goldAmount;

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
        UpdateGoldUI();
    }



    private void OnEnable()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory_Player>();

        inventory?.RebindPlayer();
        UpdateUI();
    }

    // �C���x���g��UI������I�ɍX�V�i���C�A�E�g������č\�z�j
    public void RefreshInventoryUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void UpdateGoldUI()
    {
        goldAmount.text = inventory.gold.ToString("N0");
    }
}
