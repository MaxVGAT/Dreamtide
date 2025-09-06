using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item List", fileName = "List of Items - ")]
public class ItemListDataSO : ScriptableObject
{
    public Item_DataSO[] itemList;
}
