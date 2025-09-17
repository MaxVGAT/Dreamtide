using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item List", fileName = "List of Items - ")]
public class ItemListDataSO : ScriptableObject
{
    public Item_DataSO[] itemList;

    public Item_DataSO GetItemData(string saveID)
    {
        return itemList.FirstOrDefault(item => item != null && item.saveID == saveID);
    }
}
