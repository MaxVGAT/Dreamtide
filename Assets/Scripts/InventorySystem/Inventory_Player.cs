using System;
using System.Collections.Generic;
using UnityEngine;

// プレイヤー用インベントリ管理クラス
public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed; // クイックスロット使用時イベント

    public Entity_Player player { get; private set; } // プレイヤー参照
    public List<Inventory_EquipmentSlot> equipList;   // 装備スロットリスト
    public Inventory_Storage storage { get; private set; }

    [Header("Quick Item Slots")]
    public Inventory_Item[] quickItems = new Inventory_Item[2]; // クイックスロット

    [Header("Gold Infos")]
    public int gold = 10000; // 初期ゴールド

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Entity_Player>(); // プレイヤー参照取得
    }

    public void SetQuickItemsInSlot(int slotNumber, Inventory_Item itemToSet)
    {
        quickItems[slotNumber - 1] = itemToSet;
        TriggerUpdateUI(); // UI更新
    }

    public void RebindPlayer() => player = GetComponent<Entity_Player>();

    public void TryUseQuickItemInSlot(int passedSlotNumber)
    {
        int slotNumber = passedSlotNumber - 1;
        var itemToUse = quickItems[slotNumber];

        if (itemToUse == null) return;

        TryUseItem(itemToUse);

        if (FindItem(itemToUse) == null)
            quickItems[slotNumber] = FindSameItem(itemToUse); // 同種アイテムに置き換え

        TriggerUpdateUI();
        OnQuickSlotUsed?.Invoke(slotNumber);
    }

    // アイテム使用または装備処理
    public void TryEquipItem(Inventory_Item item)
    {
        if (item.itemData.itemType == Item_Type.Consumables)
        {
            UseConsumable(item);
            return; // 消費アイテムは装備処理不要
        }

        Inventory_Item inventoryItem = FindItem(item);
        List<Inventory_EquipmentSlot> matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);
        if (matchingSlots.Count == 0) return; // 装備可能スロットなし

        foreach (var slot in matchingSlots)
        {
            if (!slot.HasItem())
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        // スロットが埋まっている場合、先頭を交換
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equippedItem;
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    public void TryUseItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item);
        if (inventoryItem == null) return;

        if (inventoryItem.itemData.itemType == Item_Type.Consumables)
        {
            if (inventoryItem.itemEffect != null && inventoryItem.itemEffect.CanBeUsed(player))
                UseConsumable(inventoryItem);

            return; // 装備処理に進まない
        }

        TryEquipItem(item);
    }

    private void UseConsumable(Inventory_Item consumable)
    {
        consumable.itemEffect.ExecuteEffect(player);
        RemoveOneItem(consumable); // 消費後在庫減少
    }

    // アイテム装備
    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();
        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.stats); // ステータス適用
        slot.equippedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveOneItem(itemToEquip); // インベントリから削除
    }

    // アイテム脱着
    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (!CanAddItem(itemToUnequip) && !replacingItem)
        {
            Debug.Log("インベントリに余裕なし、脱着不可");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        var slotToUnequip = equipList.Find(slot => slot.equippedItem == itemToUnequip);
        if (slotToUnequip != null)
            slotToUnequip.equippedItem = null;

        itemToUnequip.RemoveModifiers(player.stats);
        itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip); // インベントリに戻す
    }

    // データ保存
    public override void SaveData(ref GameData data)
    {
        data.gold = gold;
        data.inventory.Clear();
        data.equippedItems.Clear();

        foreach (var item in itemList)
        {
            if (item == null || item.itemData == null || string.IsNullOrEmpty(item.itemData.saveID))
                continue;

            string saveID = item.itemData.saveID;
            if (item.itemData.maxStackSize > 1)
            {
                if (!data.inventory.ContainsKey(saveID))
                    data.inventory[saveID] = 0;
                data.inventory[saveID] += item.stackSize;
            }
            else
            {
                if (!data.inventory.ContainsKey(saveID))
                    data.inventory[saveID] = 0;
                data.inventory[saveID] += 1;
            }
        }

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
            {
                string saveID = slot.equippedItem.itemData.saveID;
                data.equippedItems[saveID] = slot.slotType;
            }
        }
    }

    // データ読み込み
    public override void LoadData(GameData data)
    {
        gold = data.gold;
        itemList.Clear();

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
            {
                slot.equippedItem.RemoveModifiers(player.stats);
                slot.equippedItem.RemoveItemEffect();
                slot.equippedItem = null;
            }
        }

        // インベントリアイテム復元
        foreach (var item in data.inventory)
        {
            string saveID = item.Key;
            int savedStack = item.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            if (itemData == null) continue;

            if (itemData.maxStackSize > 1)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData) { stackSize = savedStack };
                AddItem(itemToLoad);
            }
            else
            {
                for (int i = 0; i < savedStack; i++)
                {
                    Inventory_Item itemToLoad = new Inventory_Item(itemData) { stackSize = 1 };
                    AddItem(itemToLoad);
                }
            }
        }

        // 装備アイテム復元
        foreach (var entry in data.equippedItems)
        {
            string saveID = entry.Key;
            Item_Type loadedSlotType = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            if (itemData == null) continue;

            Inventory_Item itemToLoad = new Inventory_Item(itemData);
            var slot = equipList.Find(slot => slot.slotType == loadedSlotType && !slot.HasItem());

            if (slot == null) continue;

            slot.equippedItem = itemToLoad;
            slot.equippedItem.AddModifiers(player.stats);
            slot.equippedItem.AddItemEffect(player);
        }

        TriggerUpdateUI(); // UI更新
    }
}
