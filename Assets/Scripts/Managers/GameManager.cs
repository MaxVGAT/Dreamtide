using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        yield return null; // Wait for scene initialization
        yield return StartCoroutine(WaitForPlayerAndComponents());

        var data = SaveManager.instance?.GetGameData();

        // Restore active checkpoint
        if (data != null && data.savedCheckpoint != Vector3.zero)
        {
            var player = Entity_Player.instance;
            if (player != null)
            {
                player.TeleportPlayer(data.savedCheckpoint);

                // Find the checkpoint closest to savedCheckpoint
                var nearestCheckpoint = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None)
                    .OrderBy(cp => Vector3.Distance(cp.GetRespawnPosition(), data.savedCheckpoint))
                    .FirstOrDefault();

                if (nearestCheckpoint != null)
                {
                    // Activate it so the GameManager knows about it
                    nearestCheckpoint.ActivateCheckpoint(true);
                }
            }
        }

        yield return StartCoroutine(HandleUIAndSaveSystem());

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

    private IEnumerator ReenableWaypoint(Object_Waypoint waypoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        waypoint.SetTriggerState(true);
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

        // Save active checkpoint to file
        var data = SaveManager.instance?.GetGameData();
        if (data != null)
        {
            data.savedCheckpoint = GetActiveCheckpoint()?.GetRespawnPosition() ?? player.transform.position;
            SaveManager.instance?.SaveGame();
        }

        // Reload current scene
        string currentScene = SceneManager.GetActiveScene().name;
        ChangeScene(currentScene, Respawn_Type.NonSpecific);

        Debug.Log("[Respawn] Player died — scene reloaded for full reset");
    }

    private Vector3 GetNewPlayerPosition(Respawn_Type type)
    {
        if (type != Respawn_Type.NonSpecific) return Vector3.zero;

        var player = Entity_Player.instance;
        if (player == null) return Vector3.zero;

        Vector3 deathPosition = player.transform.position;

        var data = SaveManager.instance.GetGameData();

        // All unlocked checkpoints
        var unlockedCheckpoints = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None)
            .Where(cp => data.unlockedCheckpoints.TryGetValue(cp.GetCheckpointId(), out bool unlocked) && unlocked)
            .Select(cp => cp.GetRespawnPosition())
            .ToList();

        // All "Enter" waypoints
        var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
            .Where(wp => wp.GetWaypointType() == Respawn_Type.Enter)
            .Select(wp => wp.GetPositionAndSetTriggerFalse())
            .ToList();

        var positions = unlockedCheckpoints.Concat(enterWaypoints).ToList();
        if (positions.Count == 0) return Vector3.zero;

        // Return closest to death
        return positions.OrderBy(pos => Vector3.Distance(pos, deathPosition)).First();
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

    #endregion
}
