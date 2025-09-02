using System;
using System.Collections.Generic;
using UnityEngine;

// インベントリ管理の基底クラス
public class Inventory_Base : MonoBehaviour
{
    // インベントリ内容が変わったときに呼ばれるイベント
    public event Action OnInventoryChange;

    [Header("インベントリ設定")]
    public int maxInventorySize = 10; // 最大アイテム数
    public List<Inventory_Item> itemList = new List<Inventory_Item>(); // 所持アイテムリスト

    protected virtual void Awake()
    {
        // 必要に応じて継承先で初期化
    }

    // アイテムを追加できるか
    public bool CanAddItem() => itemList.Count < maxInventorySize;

    // スタック可能な同一アイテムを探す
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

    // アイテムを追加する
    public void AddItem(Inventory_Item itemToAdd)
    {
        Inventory_Item itemInInventory = FindStackable(itemToAdd);

        if (itemInInventory != null)
            itemInInventory.AddStack(); // スタックに追加
        else
            itemList.Add(itemToAdd); // 新規追加

        OnInventoryChange?.Invoke(); // イベント通知
    }

    // アイテムを削除する
    public void RemoveItem(Inventory_Item itemToRemove)
    {
        itemList.Remove(FindItem(itemToRemove.itemData));
        OnInventoryChange?.Invoke(); // イベント通知
    }

    // アイテムをデータから検索
    public Inventory_Item FindItem(Item_DataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }
}
