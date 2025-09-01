using System;
using UnityEngine;

// インベントリ内のアイテムデータ管理クラス
[Serializable]
public class Inventory_Item
{
    private string itemID; // アイテムのユニークID（スタックや修飾子用）

    public Item_DataSO itemData; // 元データScriptableObject
    public int stackSize = 1;    // 現在のスタック数

    public ItemModifier[] modifiers { get; private set; } // 装備効果の修飾子

    // コンストラクタ：ScriptableObjectから生成
    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;

        modifiers = EquipmentData()?.modifiers; // 装備データがあれば修飾子を取得

        itemID = itemData.itemName + " - " + Guid.NewGuid(); // ユニークID生成
    }

    // プレイヤーに修飾子を適用
    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemID);
        }
    }

    // プレイヤーから修飾子を削除
    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stats statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemID);
        }
    }

    // Equipment_DataSOへのキャスト（装備アイテムの場合）
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }

    // スタック可能か判定
    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // スタックを増やす
    public void AddStack() => stackSize++;

    // スタックを減らす
    public void RemoveStack() => stackSize--;
}
