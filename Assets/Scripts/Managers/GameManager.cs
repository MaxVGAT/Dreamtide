using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;

    [Header("UI References")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject menuUI;

    [Header("Scene Management")]
    [SerializeField] private string defaultGameScene = "IntroScene";
    private bool isChangingScene = false;
    private Respawn_Type lastRespawnType = Respawn_Type.NonSpecific;
    private bool dataLoaded = false;
    private string lastScenePlayed;

    #region Initialization

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
        Entity_Player.OnPlayerDeathFinished += OnPlayerDeath;
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Scene Management

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        // Validate scene name
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"[GameManager] Invalid scene name: '{sceneName}'. Using default scene.");
            sceneName = defaultGameScene;
        }

        if (isChangingScene)
        {
            Debug.LogWarning($"[GameManager] Already changing scenes. Ignoring request for: {sceneName}");
            return;
        }

        Debug.Log($"[GameManager] Changing to scene: '{sceneName}' with respawn type: {respawnType}");

        isChangingScene = true;
        lastRespawnType = respawnType;
        SaveProgress();
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(HandleFadeOut());

        // Validate scene exists
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[GameManager] Scene '{sceneName}' not found in Build Settings! Using default scene.");
            sceneName = defaultGameScene;

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[GameManager] Default scene '{sceneName}' also not found! Aborting scene change.");
                isChangingScene = false;
                yield break;
            }
        }

        // Load scene
        yield return StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator HandleFadeOut()
    {
        UI_Fade fade = FindFadeScreen();
        if (fade == null) yield break;

        fade.DoFadeOut();
        float timeout = 0f;

        while (fade.fadeEffectCo != null && timeout < 3f)
        {
            timeout += Time.unscaledDeltaTime;
            yield return null;
        }

        if (timeout >= 3f)
        {
            Debug.LogWarning("[GameManager] Fade out timed out!");
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log($"[GameManager] Starting async load of: {sceneName}");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        float timeout = 0f;

        while (!asyncLoad.isDone && timeout < 20f)
        {
            timeout += Time.unscaledDeltaTime;
            yield return null;
        }

        if (timeout >= 20f)
        {
            Debug.LogError($"[GameManager] Scene loading timed out: {sceneName}");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            isChangingScene = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");

        if (scene.name == "MainMenu")
            StartCoroutine(SetupMainMenu());
        else
            StartCoroutine(SetupGameScene());
    }

    private IEnumerator SetupMainMenu()
    {
        yield return null;

        // Reset state
        isChangingScene = false;
        lastRespawnType = Respawn_Type.NonSpecific;

        // Setup UI
        SetupMainMenuUI();

        // Setup settings
        ShowHideSettings settings = FindFirstObjectByType<ShowHideSettings>();
        if (settings != null)
        {
            if (settings.mainMenuGroup == null) settings.AssignCanvasGroups();
            settings.HandleSettingsMainMenu();
        }

        // Start music
        SoundManager.instance?.StartBGM("MainMenu");

        // Fade in
        yield return StartCoroutine(HandleFadeIn());

        Debug.Log("[GameManager] Main Menu setup complete");
    }

    private IEnumerator SetupGameScene()
    {
        // Wait for data
        yield return StartCoroutine(WaitForDataLoaded());

        // Wait for player
        yield return StartCoroutine(WaitForPlayer());

        Entity_Player player = Entity_Player.instance;
        if (player == null)
        {
            Debug.LogError("[GameManager] Player not found after waiting!");
            isChangingScene = false;
            yield break;
        }

        // Position player
        PositionPlayer(player);

        yield return new WaitForSeconds(0.2f);

        // Setup UI and save system
        SetupGameUI();

        // Fade in
        yield return StartCoroutine(HandleFadeIn());

        isChangingScene = false;
        Debug.Log("[GameManager] Game scene setup complete");
    }

    private IEnumerator WaitForDataLoaded()
    {
        float timeout = 0f;
        while (!dataLoaded && timeout < 5f)
        {
            timeout += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!dataLoaded)
        {
            Debug.LogWarning("[GameManager] Data loading timed out");
        }
    }

    private IEnumerator WaitForPlayer()
    {
        float timeout = 0f;
        while (timeout < 5f)
        {
            if (Entity_Player.instance != null &&
                Entity_Player.instance.GetComponent<Player_SkillManager>() != null)
            {
                break;
            }
            timeout += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void PositionPlayer(Entity_Player player)
    {
        Vector3 target = player.transform.position;

        if (lastRespawnType != Respawn_Type.NonSpecific)
        {
            Object_Waypoint waypoint = FindWaypoint(lastRespawnType);
            if (waypoint != null)
            {
                target = waypoint.GetPositionAndSetTriggerFalse();
                Debug.Log($"[GameManager] Positioning player at waypoint: {lastRespawnType}");
            }
        }

        player.TeleportPlayer(target);
    }

    private void SetupMainMenuUI()
    {
        inGameUI = GameObject.Find("InGameUI");
        menuUI = GameObject.Find("MenuUI");

        if (inGameUI != null) inGameUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);
    }

    private void SetupGameUI()
    {
        UI ui = FindAnyObjectByType<UI>();
        if (ui == null) return;

        bool menuWasOpen = ui.IsMenuOpen();
        if (!menuWasOpen) ui.ToggleUI();

        SaveManager.instance?.RefreshAndLoad();

        if (!menuWasOpen) ui.ToggleUI();
    }

    private IEnumerator HandleFadeIn()
    {
        UI_Fade fade = FindFadeScreen();
        if (fade == null) yield break;

        fade.DoFadeIn();
        float timeout = 0f;

        while (fade.fadeEffectCo != null && timeout < 3f)
        {
            timeout += Time.unscaledDeltaTime;
            yield return null;
        }

        if (timeout >= 3f)
        {
            Debug.LogWarning("[GameManager] Fade in timed out!");
        }
    }

    private UI_Fade FindFadeScreen()
    {
        UI_Fade fade = FindFirstObjectByType<UI_Fade>();
        if (fade == null)
        {
            GameObject fadeObj = GameObject.Find("FadeScreen");
            if (fadeObj != null) fade = fadeObj.GetComponent<UI_Fade>();
        }
        return fade;
    }

    #endregion

    #region Player Death

    private void OnPlayerDeath()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui?.ToggleGameOverNoControls();
    }

    public void RespawnFromUI()
    {
        if (Entity_Player.instance == null) return;

        UI ui = FindFirstObjectByType<UI>();
        if (ui != null)
        {
            ui.ToggleGameOverNoControls();
            ui.ToggleUI();
        }

        SaveProgress();
        Debug.Log($"[GameManager] Player respawned at {Entity_Player.instance.transform.position}");
    }

    #endregion

    #region Save / Load

    public void SaveProgress()
    {
        GameData data = SaveManager.instance?.GetGameData();
        if (data == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
        {
            data.lastScenePlayed = currentScene;
        }

        SaveManager.instance?.SaveGame();
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = !string.IsNullOrEmpty(data.lastScenePlayed) ? data.lastScenePlayed : defaultGameScene;
        dataLoaded = true;
        Debug.Log($"[GameManager] Data loaded. Last scene: '{lastScenePlayed}'");
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
        {
            data.lastScenePlayed = currentScene;
        }
    }

    #endregion

    #region Waypoints

    private Object_Waypoint FindWaypoint(Respawn_Type type)
    {
        Object_Waypoint[] waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);
        foreach (Object_Waypoint waypoint in waypoints)
        {
            if (waypoint.GetWaypointType() == type) return waypoint;
        }
        return null;
    }

    public Object_Waypoint GetWaypoint(Respawn_Type type) => FindWaypoint(type);

    #endregion

    #region Public Interface

    public void GoMainMenuButton()
    {
        Debug.Log("[GameManager] Going to Main Menu");
        ChangeScene("MainMenu", Respawn_Type.NonSpecific);
    }

    public void ContinuePlay()
    {
        string sceneToLoad = GetLastPlayedScene();
        Debug.Log($"[GameManager] Continue play to scene: '{sceneToLoad}'");
        ChangeScene(sceneToLoad, Respawn_Type.NonSpecific);
    }

    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[GameManager] Restarting current scene: '{currentScene}'");
        ChangeScene(currentScene, Respawn_Type.NonSpecific);
    }

    private string GetLastPlayedScene()
    {
        // Try to get from save data first
        string savedScene = SaveManager.instance?.GetGameData()?.lastScenePlayed;

        if (!string.IsNullOrEmpty(savedScene))
        {
            return savedScene;
        }

        // Fallback to instance variable
        if (!string.IsNullOrEmpty(lastScenePlayed))
        {
            return lastScenePlayed;
        }

        // Final fallback to default
        Debug.LogWarning($"[GameManager] No saved scene found, using default: '{defaultGameScene}'");
        return defaultGameScene;
    }

    #endregion
}