using System.Collections.Generic;
using UnityEngine;

// �v���C���[��p�C���x���g���i�����Ǘ��܂ށj
public class Inventory_Player : Inventory_Base
{
    private Entity_Player player; // �v���C���[�̃X�e�[�^�X�Q��
    public List<Inventory_EquipmentSlot> equipList; // �����X���b�g���X�g

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>(); // �v���C���[�̃X�e�[�^�X�擾
    }

    // �A�C�e���𑕔����悤�Ƃ���
    public void TryEquipItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item.itemData); // �C���x���g����̃A�C�e���擾
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // �󂫃X���b�g��T���đ���
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        // �󂫂��Ȃ���΍ŏ��̃X���b�g�̃A�C�e���Ɠ���ւ�
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equippedItem;

        EquipItem(inventoryItem, slotToReplace);
        UnequipItem(itemToUnequip);
    }

    // �w��X���b�g�ɃA�C�e���𑕔�
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.stats); // �X�e�[�^�X�ɏC���q��K�p
        slot.equippedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveItem(itemToEquip); // �C���x���g������폜
    }

    // �A�C�e���𑕔����
    public void UnequipItem(Inventory_Item itemToUnequip)
    {
        if (CanAddItem() == false)
        {
            Debug.Log("�C���x���g���ɋ󂫂�����܂���");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        // �����X���b�g����폜
        var slotToUnequip = equipList.Find(slot => slot.equippedItem == itemToUnequip);

        if (slotToUnequip != null)
            slotToUnequip.equippedItem = null;

        itemToUnequip.RemoveModifiers(player.stats); // �X�e�[�^�X�C���q��폜
        itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip); // �C���x���g���ɖ߂�
    }
}
