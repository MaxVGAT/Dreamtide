using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item List", fileName = "List of Items - ")]
public class ItemListDataSO : ScriptableObject
{
    public Item_DataSO[] itemList; // 登録されている全アイテムデータ

    public Item_DataSO GetItemData(string saveID)
    {
        // saveID に一致するアイテムデータを返す（存在しなければ null）
        return itemList.FirstOrDefault(item => item != null && item.saveID == saveID);
    }
}
