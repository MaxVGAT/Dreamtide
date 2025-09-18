using TMPro;
using UnityEngine;

public class ShopDialogColor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText; // 表示するテキストUI
    [SerializeField] private Color nameColor;            // 商人名の文字色
    [SerializeField] private string merchantName;        // 商人名
    [SerializeField] private string npcSentence;         // NPCのセリフ

    private void Start()
    {
        SetTextColor(); // 開始時にテキスト色を設定
    }

    private void SetTextColor()
    {
        // 色を16進数に変換してテキストに適用
        string hexColor = ColorUtility.ToHtmlStringRGB(nameColor);
        dialogText.text = $"<color=#{hexColor}>{merchantName}</color>:" + $"{npcSentence}";
    }
}
