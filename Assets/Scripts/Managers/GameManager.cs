using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //==================================================
    // SINGLETON INSTANCE
    //==================================================
    public static GameManager Instance { get; private set; }

    //==================================================
    // CURSOR SETTINGS
    //==================================================
    public Texture2D customCursor;
    public Vector2 hotspot = Vector2.zero;

    //==================================================
    // NEXT SCENE & SPAWN POINT
    //==================================================

    //==================================================
    // SPAWN POINT REFERENCES
    //==================================================

    //==================================================
    // UI ELEMENTS
    //==================================================
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pauseDimmerPanel;
    [SerializeField] private GameObject pauseText;

    //==================================================
    // GAME STATE FLAGS
    //==================================================

    private bool isPaused = false;

    //==================================================
    // UNITY EVENTS
    //==================================================
    private void Awake()
    {
        Cursor.SetCursor(customCursor, hotspot, CursorMode.Auto);
    }

    private void Start()
    {

    }

    private void Update()
    {
       
    }

    //==================================================
    // GAME CONTROL METHODS
    //==================================================
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame(string sceneName, string spawnPoint)
    {
        
    }

    //==================================================
    // RESPAWN PLAYER
    //==================================================
    public void RespawnPlayer()
    {

    }

    //==================================================
    // PAUSE AND CLEAR GAME
    //==================================================
    public void PauseGame()
    {
        // Toggle pause state and update UI, timescale, and SFX
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        SoundManager.Instance.PlayPauseSFX();
    }
}
