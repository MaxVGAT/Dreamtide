using UnityEngine;
using UnityEngine.UI;

public class UI_ScrollToTopOnStart : MonoBehaviour
{
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        scrollRect.verticalNormalizedPosition = 1f; // force at top
    }
}
