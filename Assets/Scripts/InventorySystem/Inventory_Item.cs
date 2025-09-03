using System;
using UnityEngine;

// �C���x���g����̃A�C�e���f�[�^�Ǘ��N���X
[Serializable]
public class Inventory_Item
{
    private string itemID; // �A�C�e���̃��j�[�NID�i�X�^�b�N��C���q�p�j

    public Item_DataSO itemData; // ���f�[�^ScriptableObject
    public int stackSize = 1;    // ���݂̃X�^�b�N��

    public ItemModifier[] modifiers { get; private set; } // �������ʂ̏C���q
    public Item_EffectDataSO itemEffect;

    // �R���X�g���N�^�FScriptableObject���琶��
    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;

        modifiers = EquipmentData()?.modifiers; // �����f�[�^������ΏC���q��擾
        itemEffect = itemData.itemEFfect;

        itemID = itemData.itemName + " - " + Guid.NewGuid(); // ���j�[�NID����
    }

    // �v���C���[�ɏC���q��K�p
    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemID);
        }
    }

    // �v���C���[����C���q��폜
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemID);
        }
    }

    public void AddItemEffect(Entity_Player player) => itemEffect?.Subscribe (player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe ();

    // Equipment_DataSO�ւ̃L���X�g�i�����A�C�e���̏ꍇ�j
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }

    // �X�^�b�N�\������
    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // �X�^�b�N�𑝂₷
    public void AddStack() => stackSize++;

    // �X�^�b�N����炷
    public void RemoveStack() => stackSize--;
}
