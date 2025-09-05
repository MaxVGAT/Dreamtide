using System;
using System.Collections.Generic;
using UnityEngine;

// �C���x���g���Ǘ��̊��N���X
public class Inventory_Base : MonoBehaviour
{
    // �C���x���g����e���ς�����Ƃ��ɌĂ΂��C�x���g
    public event Action OnInventoryChange;

    [Header("�C���x���g���ݒ�")]
    public int maxInventorySize = 10; // �ő�A�C�e����
    public List<Inventory_Item> itemList = new List<Inventory_Item>(); // �����A�C�e�����X�g

    protected virtual void Awake()
    {
        // �K�v�ɉ����Čp����ŏ�����
    }

    // �A�C�e����ǉ��ł��邩
    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        bool hasStackable = FindStackable(itemToAdd) != null;

        return hasStackable || itemList.Count < maxInventorySize;
    }

    // �X�^�b�N�\�ȓ���A�C�e����T��
    public Inventory_Item FindStackable(Inventory_Item itemToAdd)
    {
        List<Inventory_Item> stackableItems = itemList.FindAll(item => item.itemData == itemToAdd.itemData);

        foreach (var stackableItem in stackableItems)
        {
            if (stackableItem.CanAddStack())
                return stackableItem;
        }

        return null;
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumable = itemList.Find(item => item == itemToUse);

        if (consumable == null)
            return;

        consumable.itemEffect.ExecuteEffect();

        if (consumable.stackSize > 1)
            consumable.RemoveStack();
        else
            RemoveOneItem(consumable);

        OnInventoryChange?.Invoke();
    }

    // �A�C�e����ǉ�����
    public void AddItem(Inventory_Item itemToAdd)
    {
        Inventory_Item itemInInventory = FindStackable(itemToAdd);

        if (itemInInventory != null)
            itemInInventory.AddStack(); // �X�^�b�N�ɒǉ�
        else
            itemList.Add(itemToAdd); // �V�K�ǉ�

        OnInventoryChange?.Invoke(); // �C�x���g�ʒm
    }

    // �A�C�e����폜����
    public void RemoveOneItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);

        if(itemInInventory.stackSize > 1)
            itemInInventory.RemoveStack();
        else
            itemList.Remove(itemToRemove);

            OnInventoryChange?.Invoke(); // �C�x���g�ʒm
    }

    // �A�C�e����f�[�^���猟��
    public Inventory_Item FindItem(Item_DataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();
}
