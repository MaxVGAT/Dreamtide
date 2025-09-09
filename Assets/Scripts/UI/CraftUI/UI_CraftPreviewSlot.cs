using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviewSlot : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialNameValue;

    public void SetupPreviewSlot(Item_DataSO itemData, int availableAmount, int requiredAmount)
    {
        string availableColor = availableAmount >= requiredAmount ? "#00FF00" : "#FF0000";

        materialIcon.sprite = itemData.itemIcon;
        materialNameValue.text = $"{itemData.itemName} - <color={availableColor}>{availableAmount}</color> / {requiredAmount}";
    }
}
