using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UI_Options : MonoBehaviour
{
    private Entity_Player player;
    [SerializeField] private Toggle healthBarToggle;

    private void Start()
    {
        player = FindFirstObjectByType<Entity_Player>();

        if (healthBarToggle != null)
        {
            healthBarToggle.onValueChanged.AddListener(OnHealthBarToggle);
        }
    }

    private void OnHealthBarToggle(bool isOn)
    {
        if (player != null && player.health != null)
        {
            player.health.EnableHealthBar(isOn);
        }
    }

    public void GoMainMenuButton()
    {
        Debug.Log("[UI_Options] GoMainMenuButton called");

        if (GameManager.instance != null)
        {
            GameManager.instance.GoMainMenuButton();
        }
        else
        {
            Debug.LogWarning("[UI_Options] GameManager instance is null, loading MainMenu directly");
            SceneManager.LoadScene("MainMenu");
        }
    }
}