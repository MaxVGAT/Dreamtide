using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Equipment item", fileName = "Equipment data - ")]
public class Equipment_DataSO : Item_DataSO
{
    [Header("Item modifiers")]
    public ItemModifier[] modifiers;
}

[Serializable]
public class ItemModifier
{
    public StatType statType;
    public float value;
}
