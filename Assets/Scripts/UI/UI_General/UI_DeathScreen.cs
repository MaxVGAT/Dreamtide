using UnityEngine;
using System.Collections;

public class UI_DeathScreen : MonoBehaviour
{
    public void RespawnButton()
    {
        GameManager.instance.RestartCurrentScene();
    }

    public void GoMainMenuButton()
    {
        StartCoroutine(GoMainMenuNextFrame());
    }

    private IEnumerator GoMainMenuNextFrame()
    {
        yield return null;

        // Safety check before calling GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.GoMainMenuButton();
        }
        else
        {
            Debug.LogWarning("[UI_DeathScreen] GameManager instance is null, cannot go to main menu");
            // Fallback: directly load main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}