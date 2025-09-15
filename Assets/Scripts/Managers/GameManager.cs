using System.Collections;
using System.Linq;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;

    private string lastScenePlayed;

    [Header("Scene Transition")]
    private Respawn_Type lastRespawnType;
    private bool isChangingScene = false;

    [Header("Checkpoint System")]
    public string activeCheckpointID;

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

    public Object_Checkpoints GetActiveCheckpoint()
    {
        if (string.IsNullOrEmpty(activeCheckpointID)) return null;
        return FindCheckpoint(activeCheckpointID);
    }

    public Object_Waypoint GetWaypoint(Respawn_Type type)
    {
        return FindWaypoint(type);
    }

    public string ActiveCheckpointID => activeCheckpointID;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(HandleSceneSetup());
    }

    private IEnumerator HandleSceneSetup()
    {
        // Wait for the scene to initialize
        yield return null;

        // Wait for player instance and required components
        while (Entity_Player.instance == null)
            yield return null;

        var player = Entity_Player.instance;

        while (player.GetComponent<Player_SkillManager>() == null)
            yield return null;

        // Restore player position
        var data = SaveManager.instance?.GetGameData();
        if (data != null)
        {
            Vector3 targetPosition;

            // Use nearest active checkpoint if it exists
            var checkpoint = GetActiveCheckpoint();
            if (checkpoint != null)
                targetPosition = checkpoint.GetRespawnPosition();
            else
                targetPosition = data.savedCheckpoint != Vector3.zero
                    ? data.savedCheckpoint
                    : player.transform.position;

            // Teleport player once, AFTER the player is fully initialized
            player.TeleportPlayer(targetPosition);
        }

        // Handle UI and save system
        UI ui = null;
        while ((ui = FindAnyObjectByType<UI>()) == null)
            yield return null;

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

        isChangingScene = false;
    }


    private IEnumerator WaitForPlayerAndComponents()
    {
        while (Entity_Player.instance == null)
            yield return null;

        while (Entity_Player.instance.GetComponent<Player_SkillManager>() == null)
            yield return null;
    }

    private IEnumerator HandleUIAndSaveSystem()
    {
        // Wait until UI exists
        UI ui = null;
        while ((ui = FindAnyObjectByType<UI>()) == null)
            yield return null;

        // Store menu/tab state
        bool wasMenuOpen = ui.IsMenuOpen();
        UI_TabButton rememberedTab = ui.tabGroup?.selectedTab;
        int rememberedIndex = ui.tabGroup?.defaultTabIndex ?? 0;

        // Open UI if needed
        if (!wasMenuOpen) { ui.ToggleUI(); yield return null; }

        // Refresh/save
        SaveManager.instance?.RefreshAndLoad();
        yield return null;

        // Restore tab
        if (ui.tabGroup != null)
        {
            if (rememberedTab != null)
                ui.tabGroup.OnTabSelected(rememberedTab);
            else if (ui.tabGroup.tabButtons.Count > 0)
                ui.tabGroup.OnTabSelected(ui.tabGroup.tabButtons[Mathf.Clamp(rememberedIndex, 0, ui.tabGroup.tabButtons.Count - 1)]);

            yield return null;
        }

        // Close UI if we opened it
        if (!wasMenuOpen) { ui.ToggleUI(); yield return null; }
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

    #endregion

    #region Player Death and Restart

    private void OnEnable()
    {
        Entity_Player.OnPlayerDeathFinished += HandlePlayerRespawn;
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeathFinished -= HandlePlayerRespawn;
    }

    private void HandlePlayerRespawn()
    {
        var player = Entity_Player.instance;
        if (player == null)
        {
            Debug.LogWarning("[Respawn] No player instance found!");
            return;
        }

        SaveProgress();

        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        ChangeScene(currentScene, Respawn_Type.NonSpecific);

        Debug.Log("[Respawn] Player died — scene reloaded for full reset");
    }


    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        ChangeScene(currentSceneName, Respawn_Type.NonSpecific);
    }

    #endregion

    #region Checkpoint System

    public void SetActiveCheckpoint(string checkpointID)
    {
        activeCheckpointID = checkpointID;
        SaveProgress();
    }

    private Object_Checkpoints FindCheckpoint(string checkpointID)
    {
        foreach (var checkpoint in FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None))
        {
            if (checkpoint.GetCheckpointId() == checkpointID)
                return checkpoint;
        }
        return null;
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
        if (data == null) return;

        // 1. Save current scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
            data.lastScenePlayed = currentScene;

        // 2. Save active checkpoint position
        var checkpoint = GetActiveCheckpoint();
        var player = Entity_Player.instance;
        if (player != null)
            data.savedCheckpoint = checkpoint?.GetRespawnPosition() ?? player.transform.position;

        SaveManager.instance?.SaveGame();
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;

        if (string.IsNullOrEmpty(lastScenePlayed))
            lastScenePlayed = "IntroScene";
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
            return;

        data.lastScenePlayed = currentScene;
    }

    #endregion
}