using System.Collections;
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

        HandlePlayerTeleportation();

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

    private void HandlePlayerTeleportation()
    {
        TeleportToWaypoint();
        TeleportToActiveCheckpoint();
    }

    private void TeleportToWaypoint()
    {
        Object_Waypoint targetWaypoint = FindWaypoint(lastRespawnType);
        if (targetWaypoint == null) return;

        Vector3 teleportPosition = targetWaypoint.GetPositionAndSetTriggerFalse();
        Entity_Player.instance.TeleportPlayer(teleportPosition);

        StartCoroutine(ReenableWaypoint(targetWaypoint, 0.5f));
    }

    private void TeleportToActiveCheckpoint()
    {
        if (string.IsNullOrEmpty(activeCheckpointID)) return;

        Object_Checkpoints activeCheckpoint = FindCheckpoint(activeCheckpointID);
        if (activeCheckpoint != null)
            Entity_Player.instance.TeleportPlayer(activeCheckpoint.GetRespawnPosition());
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
        if (player == null) return;

        var checkpoint = GetActiveCheckpoint();
        player.RespawnAtCheckpoint(checkpoint);
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
