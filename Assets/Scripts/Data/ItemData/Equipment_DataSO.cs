using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Equipment item", fileName = "Equipment data - ")]
public class Equipment_DataSO : Item_DataSO // 装備アイテムの基本データを継承
{
    [Header("Item modifiers")]
    public ItemModifier[] modifiers; // ステータスに影響する修飾子の配列
}

[Serializable] // Unityインスペクタで表示可能にする
public class ItemModifier
{
    public StatType statType; // 影響を与えるステータスの種類
    public float value; // 変更値
}