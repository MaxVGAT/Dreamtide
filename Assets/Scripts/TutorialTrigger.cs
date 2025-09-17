using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string message; // Message for this trigger

    [SerializeField] private TutorialUI tutorialUI; // Assign in inspector

    private void Awake()
    {
        if (tutorialUI == null)
            tutorialUI = FindFirstObjectByType<TutorialUI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            tutorialUI.ShowTutorial(message);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            tutorialUI.HideTutorial();
    }
}