using System.Collections.Generic;
using UnityEngine;

// �v���C���[��p�C���x���g���i�����Ǘ��܂ށj
public class Inventory_Player : Inventory_Base
{
    private Entity_Player player; // �v���C���[�̃X�e�[�^�X�Q��
    public List<Inventory_EquipmentSlot> equipList; // �����X���b�g���X�g
    public Inventory_Storage storage;

    public int gold = 10000;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>(); // �v���C���[�̃X�e�[�^�X�擾
    }

    // �A�C�e���𑕔����悤�Ƃ���
    public void TryEquipItem(Inventory_Item item)
    {

        // --- Handle consumables first ---
        if (item.itemData.itemType == Item_Type.Consumables)
        {
            UseConsumable(item); // executes effect and reduces stack
            return; // stop here, do NOT touch equip slots
        }

        Inventory_Item inventoryItem = FindItem(item.itemData);
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // Add this check for consumables or items with no matching slots
        if (matchingSlots.Count == 0)
        {
            return;
        }

        // Rest of your existing code...
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        var slotToReplace = matchingSlots[0]; // This line won't crash now
        var itemToUnequip = slotToReplace.equippedItem;
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    public void TryUseItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item.itemData);
        if (inventoryItem == null)
            return;

        // Check if it has an effect that can be used (consumable)
        if (inventoryItem.itemEffect != null && inventoryItem.itemEffect.CanBeUsed())
            UseConsumable(inventoryItem);
        else
            TryEquipItem(item);
    }

    private void UseConsumable(Inventory_Item consumable)
    {
        consumable.itemEffect.ExecuteEffect();
        RemoveOneItem(consumable);
    }

    // �w��X���b�g�ɃA�C�e���𑕔�
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.stats); // �X�e�[�^�X�ɏC���q��K�p
        slot.equippedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveOneItem(itemToEquip); // �C���x���g������폜
    }

    // �A�C�e���𑕔����
    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
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
