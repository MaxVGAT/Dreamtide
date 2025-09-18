using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab; // ドロップ用プレハブ
    [SerializeField] private ItemListDataSO dropData;  // ドロップデータ参照

    [Header("Drop restrictions")]
    [SerializeField] private int maxRarityAmount = 1200; // 総レアリティ上限
    [SerializeField] private int maxItemsToDrop = 4;     // 最大ドロップ数

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            DropItems(); // デバッグ用: Xキーでアイテムドロップ
    }

    public virtual void DropItems()
    {
        if (dropData == null)
        {
            Debug.Log("You need to assign drop data on entity"); // ドロップデータ未設定時
            return;
        }

        List<Item_DataSO> itemsToDrop = RollDrops(); // ドロップアイテム決定
        int amountToDrop = Mathf.Min(itemsToDrop.Count, maxItemsToDrop); // 最大ドロップ数調整

        for (int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(itemsToDrop[i]); // アイテム生成
        }
    }

    protected void CreateItemDrop(Item_DataSO itemToDrop)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop); // アイテム情報設定
    }

    public List<Item_DataSO> RollDrops()
    {
        List<Item_DataSO> possibleDrops = new List<Item_DataSO>(); // 候補リスト
        List<Item_DataSO> finalDrops = new List<Item_DataSO>();    // 最終ドロップリスト
        float maxRarityAmount = this.maxRarityAmount;              // 上限初期化

        // ステップ1: レアリティ・確率で候補を抽選
        foreach (var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();

            if (Random.Range(0, 100) <= dropChance)
                possibleDrops.Add(item);
        }

        // ステップ2: レアリティ順にソート（高→低）
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarityScale).ToList();

        // ステップ3: レアリティ上限に達するまで追加
        foreach (var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarityScale)
            {
                finalDrops.Add(item);
                maxRarityAmount -= item.itemRarityScale;
            }
        }

        return finalDrops;
    }
}
