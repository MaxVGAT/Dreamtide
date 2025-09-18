using System.Collections.Generic;
using UnityEngine;

// 商人用インベントリクラス
public class Inventory_Merchant : Inventory_Base
{
    public Inventory_Player inventory { get; private set; } // プレイヤーのインベントリ参照

    [SerializeField] private ItemListDataSO shopData; // 店の出品データ
    [SerializeField] private int minItemsAmount = 4;  // 最小出品数

    protected override void Awake()
    {
        base.Awake();
        FillShopList(); // 初期化時に出品アイテム生成
    }

    // アイテム購入処理
    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;

        for (int i = 0; i < amountToBuy; i++)
        {
            if (inventory.gold < itemToBuy.buyPrice) return; // 金不足で終了

            if (inventory.CanAddItem(itemToBuy))
            {
                var itemToAdd = new Inventory_Item(itemToBuy.itemData);
                inventory.AddItem(itemToAdd);
                inventory.gold -= itemToBuy.buyPrice;
                RemoveOneItem(itemToBuy); // 商人側の在庫減少
            }
            else break;
        }

        TriggerUpdateUI(); // UI更新通知
    }

    // アイテム売却処理
    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;

        for (int i = 0; i < amountToSell; i++)
        {
            int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);

            inventory.gold += sellPrice;          // プレイヤーにゴールド加算
            inventory.RemoveOneItem(itemToSell);  // プレイヤーの在庫減少
        }

        TriggerUpdateUI(); // UI更新通知
    }

    // 商人在庫生成
    public void FillShopList()
    {
        itemList.Clear();
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var itemData in shopData.itemList)
        {
            int randomizedStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomizedStack, 1, itemData.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStack;

            possibleItems.Add(itemToAdd);
        }

        int randomItemAmount = Random.Range(minItemsAmount, maxInventorySize + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);

        for (int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (CanAddItem(item))
            {
                possibleItems.Remove(item); // 重複防止
                AddItem(item);              // 商人在庫に追加
            }
        }

        TriggerUpdateUI(); // UI更新通知
    }

    // プレイヤーインベントリ設定
    public void SetInventory(Inventory_Player inventory) => this.inventory = inventory;
}
