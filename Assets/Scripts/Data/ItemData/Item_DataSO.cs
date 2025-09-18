using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Material item", fileName = "Material data - ")]
public class Item_DataSO : ScriptableObject
{
    // セーブ用一意ID（GUID）※自動生成
    public string saveID { get; private set; }

    [Header("Merchant Details")]
    public int minStackSizeAtShop = 1; // ショップに並ぶ最小スタック数
    public int maxStackSizeAtShop = 1; // ショップに並ぶ最大スタック数

    [Header("Craft Details")]
    public Inventory_Item[] craftRecipe; // このアイテムのクラフトレシピ

    [Header("Drop Details")]
    [Range(0, 1000)] public int itemRarityScale = 100; // レアリティスケール（数値が高いほど入手困難）
    [Range(0, 100)] public float dropChance;           // 実際のドロップ確率
    [Range(0, 100)] public float maxDropChance = 65f;  // ドロップ確率の上限

    [Header("Item Details")]
    public string itemName;        // アイテム名
    public Sprite itemIcon;        // アイコン画像
    public Item_Rarity itemRarity; // レアリティ区分
    public Item_Type itemType;     // アイテム種別
    public int maxStackSize = 1;   // 最大スタック数

    [Header("Item effect")]
    public Item_EffectDataSO itemEffect; // 使用効果データ

    private void OnValidate()
    {
        // エディタ上で値変更時にドロップ率を再計算
        dropChance = GetDropChance();

#if UNITY_EDITOR
        // アセットのGUIDをsaveIDとして保存
        string path = AssetDatabase.GetAssetPath(this);
        saveID = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public float GetDropChance()
    {
        // レアリティ値からドロップ率を算出し、上限で制限
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarityScale + 1) / maxRarity * 100;

        return Mathf.Min(chance, maxDropChance);
    }
}
