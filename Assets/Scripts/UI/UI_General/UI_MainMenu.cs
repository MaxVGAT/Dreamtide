using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Fade fadeScreen; // Drag your fade component here in inspector
    [SerializeField] private ParticleSystem particleEffects; // Assign in inspector
    [SerializeField] private Button playButton;

    private bool hasClicked = false;

    private void Start()
    {
        // Keep fadeScreen disabled at start
        if (fadeScreen == null)
            fadeScreen = FindFirstObjectByType<UI_Fade>();

        if (fadeScreen != null)
            fadeScreen.gameObject.SetActive(false);

        SoundManager.instance.NextBGM("music_mainMenu");
    }

    public void PlayButton()
    {
        if (hasClicked)
            return; // Ignore further clicks if already clicked

        hasClicked = true; // Mark as clicked
        if (playButton != null)
            playButton.interactable = false;

        StartCoroutine(FadeAndStartGame());
    }

    private IEnumerator FadeAndStartGame()
    {
        // Disable particle effects immediately
        if (particleEffects != null)
            particleEffects.gameObject.SetActive(false);

        // Find fade screen if not assigned
        if (fadeScreen == null)
            fadeScreen = FindFirstObjectByType<UI_Fade>();

        if (fadeScreen != null)
        {
            // Enable fade screen
            fadeScreen.gameObject.SetActive(true);

            // Immediately set alpha to 0 (transparent)
            var img = fadeScreen.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                var color = img.color;
                color.a = 0f;
                img.color = color;
            }

            // Start fade out and wait for it to complete
            fadeScreen.DoFadeOut(1f);
            while (fadeScreen.fadeEffectCo != null)
                yield return null;
        }

        // Now start the game
        GameManager.instance.ContinuePlay();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
