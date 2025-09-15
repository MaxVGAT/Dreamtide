using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;

    private string lastScenePlayed;

    [Header("Scene Transition")]
    private Respawn_Type lastRespawnType;
    private bool isChangingScene = false;

    #region Initialization

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // Clean up any event subscriptions on the old instance before destroying
            if (instance != null)
            {
                SceneManager.sceneLoaded -= instance.OnSceneLoaded;
                Entity_Player.OnPlayerDeathFinished -= instance.OnPlayerDeath;
            }

            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;

        // Clear instance if this was the active instance
        if (instance == this)
            instance = null;
    }

    public Object_Waypoint GetWaypoint(Respawn_Type type)
    {
        return FindWaypoint(type);
    }

    #endregion

    #region Scene Management

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        if (isChangingScene)
            return;

        StartSceneTransition(sceneName, respawnType);
    }

    private void StartSceneTransition(string sceneName, Respawn_Type respawnType)
    {
        isChangingScene = true;
        lastRespawnType = respawnType;

        SaveProgress();

        // Unsubscribe first to prevent any lingering subscriptions
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Only subscribe if this is the active instance
        if (instance == this)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe immediately to prevent duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Check if this GameManager instance is still valid and is the active instance
        if (this == null || instance != this || this != instance)
        {
            Debug.LogWarning("[GameManager] OnSceneLoaded called on destroyed or inactive GameManager instance - ignoring");
            return;
        }

        // Additional check to make sure we can start coroutines
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[GameManager] GameManager not active in hierarchy - cannot start coroutines");
            return;
        }

        StartCoroutine(HandleSceneSetup());
    }

    private IEnumerator HandleSceneSetup()
    {
        // Additional safety check
        if (this == null || instance != this)
        {
            Debug.LogWarning("[GameManager] HandleSceneSetup called on destroyed or inactive GameManager instance");
            yield break;
        }

        // Wait for scene initialization, player, and components
        yield return null;
        yield return StartCoroutine(WaitForPlayerAndComponents());

        var player = Entity_Player.instance;

        // Determine teleport position
        Vector3 targetPosition = player.transform.position; // Default: leave player at spawn

        // Only use waypoint if lastRespawnType is meaningful
        if (lastRespawnType != Respawn_Type.NonSpecific)
        {
            var waypoint = GetWaypoint(lastRespawnType);
            if (waypoint != null)
                targetPosition = waypoint.GetPositionAndSetTriggerFalse();
        }
        // else: keep player at default scene spawn

        // Teleport player once
        player.TeleportPlayer(targetPosition);

        // Handle UI and save as before
        yield return StartCoroutine(HandleUIAndSaveSystem());

        isChangingScene = false;

        var ui = FindFirstObjectByType<UI>();
        if (ui != null)
        {
            ui.ToggleUI();
            ui.ToggleUI();
        }

        SaveProgress();
    }

    private IEnumerator WaitForPlayerAndComponents()
    {
        while (Entity_Player.instance == null)
        {
            if (this == null || instance != this) yield break; // Safety check
            yield return null;
        }

        while (Entity_Player.instance.GetComponent<Player_SkillManager>() == null)
        {
            if (this == null || instance != this) yield break; // Safety check
            yield return null;
        }
    }

    private IEnumerator HandleUIAndSaveSystem()
    {
        UI ui = null;
        while ((ui = FindAnyObjectByType<UI>()) == null)
        {
            if (this == null || instance != this) yield break; // Safety check
            yield return null;
        }

        // Special case: MainMenu always starts closed (or default state)
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ui.ToggleUI(); // or whatever method closes credits, options, etc.
            yield break; // skip restoring previous scene UI
        }

        // Otherwise, restore previous state
        bool wasMenuOpen = ui.IsMenuOpen();
        UI_TabButton rememberedTab = ui.tabGroup?.selectedTab;
        int rememberedIndex = ui.tabGroup?.defaultTabIndex ?? 0;

        if (!wasMenuOpen)
        {
            ui.ToggleUI();
            yield return null;
        }

        SaveManager.instance?.RefreshAndLoad();
        yield return null;

        if (ui.tabGroup != null)
        {
            if (rememberedTab != null)
                ui.tabGroup.OnTabSelected(rememberedTab);
            else if (ui.tabGroup.tabButtons.Count > 0)
                ui.tabGroup.OnTabSelected(ui.tabGroup.tabButtons[Mathf.Clamp(rememberedIndex, 0, ui.tabGroup.tabButtons.Count - 1)]);

            yield return null;
        }

        if (!wasMenuOpen)
        {
            ui.ToggleUI();
            yield return null;
        }
    }

    public void ContinuePlay()
    {
        // Use saved data if available
        var data = SaveManager.instance?.GetGameData();

        // If no save or empty, fallback to default scene
        string sceneToLoad = (data != null && !string.IsNullOrEmpty(data.lastScenePlayed))
            ? data.lastScenePlayed
            : "IntroScene"; // <-- default starting scene

        // Only try to load if sceneToLoad is valid
        if (!string.IsNullOrEmpty(sceneToLoad))
            ChangeScene(sceneToLoad, Respawn_Type.NonSpecific);
        else
            Debug.LogError("[GameManager] No valid scene to load in ContinuePlay!");
    }

    public void GoMainMenuButton()
    {
        StartCoroutine(CloseUIOnBackToMenu());
    }

    public IEnumerator CloseUIOnBackToMenu()
    {
        // Load MainMenu scene
        SceneManager.LoadScene("MainMenu");

        // Wait until MainMenu scene is active
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "MainMenu");

        // Safety check
        if (this == null || instance != this) yield break;

        // Find the ShowHideSettings in the scene
        ShowHideSettings settings = FindFirstObjectByType<ShowHideSettings>();
        if (settings == null)
        {
            yield break;
        }

        // Assign CanvasGroups dynamically if needed
        if (settings.mainMenuGroup == null)
            settings.AssignCanvasGroups();

        // Hide all panels
        settings.HandleSettingsMainMenu();
    }

    #endregion

    #region Player Death and Restart

    private void OnEnable()
    {
        // Ensure no duplicate subscriptions
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
        Entity_Player.OnPlayerDeathFinished += OnPlayerDeath;
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        // Show game over UI
        var ui = FindFirstObjectByType<UI>();
        if (ui != null)
            ui.ToggleGameOverNoControls();

        // Pause gameplay if needed
        // Time.timeScale = 0f;
    }

    public void RespawnFromUI()
    {
        if (Entity_Player.instance == null) return;

        // Just respawn at current position since no checkpoints
        Vector3 respawnPos = Entity_Player.instance.transform.position;

        // Close GameOver UI
        var ui = FindFirstObjectByType<UI>();
        if (ui != null)
        {
            ui.ToggleGameOverNoControls();
            ui.ToggleUI();
        }

        SaveProgress();

        Debug.Log($"[Respawn] Player respawned at position: {respawnPos}");
    }

    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        ChangeScene(currentSceneName, Respawn_Type.NonSpecific);
    }

    #endregion

    #region Waypoint System

    private Object_Waypoint FindWaypoint(Respawn_Type respawnType)
    {
        foreach (var waypoint in FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None))
        {
            if (waypoint.GetWaypointType() == respawnType)
                return waypoint;
        }
        return null;
    }

    public void SaveProgress()
    {
        var data = SaveManager.instance?.GetGameData();
        if (data == null)
        {
            Debug.LogWarning("[Save] No GameData available to save!");
            return;
        }

        // Save the current scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
            data.lastScenePlayed = currentScene;

        SaveManager.instance?.SaveGame();

        Debug.Log($"[Save] Progress saved - Scene: {currentScene}");
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "IntroScene";

        Debug.Log($"[Load] Data loaded - Scene: {lastScenePlayed}");
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        data.lastScenePlayed = currentScene;

        Debug.Log($"[Save] Data saved - Scene: {currentScene}");
    }

    #endregion
}