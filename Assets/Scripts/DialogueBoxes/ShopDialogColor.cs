using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class ShopDialogColor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Color nameColor;
    [SerializeField] private string merchantName;
    [SerializeField] private string npcSentence;

    private void Start()
    {
        SetTextColor();
    }

    private void SetTextColor()
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(nameColor);
        dialogText.text = $"<color=#{hexColor}>{merchantName}</color>:" + $"{npcSentence}";
    }
}
