using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;

    public void ShowTutorial(string message)
    {
        panel.SetActive(true);
        messageText.text = ColorFirstWord(message);
    }

    public void HideTutorial()
    {
        if (panel == null)
            return;

        panel.SetActive(false);
    }
    private string ColorFirstWord(string message, string keyColor = "#FF5555")
    {
        int firstSpace = message.IndexOf(' ');
        if (firstSpace < 0) firstSpace = message.Length;

        string firstWord = message.Substring(0, firstSpace);
        string rest = message.Substring(firstSpace).TrimStart();

        return $"<color={keyColor}>{firstWord}</color> {rest}";
    }
}