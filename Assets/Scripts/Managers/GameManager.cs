using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Respawn_Type lastRespawnType;
    private bool isChangingScene = false;
    private bool skipNextUIOpen = false; // Track if we need to skip UI opening

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        if (isChangingScene) return;

        isChangingScene = true;
        lastRespawnType = respawnType;
        skipNextUIOpen = false; // Reset flag

        StartCoroutine(ChangeSceneCo(sceneName));
    }

    private IEnumerator ChangeSceneCo(string sceneName)
    {
        // Save game before leaving current scene
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
            yield return new WaitForSeconds(0.1f); // Brief wait for save
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isChangingScene)
        {
            StartCoroutine(HandleSceneTransition());
        }
    }

    private IEnumerator HandleSceneTransition()
    {
        yield return null; // Wait one frame for scene initialization

        // Find the target waypoint
        Object_Waypoint targetWaypoint = GetWaypoint(lastRespawnType);

        // Wait for player to exist
        while (Entity_Player.instance == null)
        {
            yield return null;
        }

        // Wait for skill manager
        Player_SkillManager skillManager = null;
        while (skillManager == null)
        {
            skillManager = Entity_Player.instance.GetComponent<Player_SkillManager>();
            yield return null;
        }

        // Teleport player first
        if (targetWaypoint != null)
        {
            Entity_Player.instance.TeleportPlayer(targetWaypoint.GetRespawnPosition());

            // Enable the waypoint after a short delay
            StartCoroutine(EnableWaypointAfterDelay(targetWaypoint, 0.5f));
        }

        // Handle UI and save loading with proper state management
        yield return StartCoroutine(HandleUISaveLoading());

        // Reset flag
        isChangingScene = false;
    }

    private IEnumerator HandleUISaveLoading()
    {
        var uiScript = FindAnyObjectByType<UI>();
        if (uiScript != null)
        {
            // Store the current menu state BEFORE any operations
            bool wasMenuOpen = uiScript.IsMenuOpen();

            // Store tab state if available
            UI_TabButton rememberedSelectedTab = null;
            int rememberedDefaultIndex = 0;

            if (uiScript.tabGroup != null)
            {
                rememberedSelectedTab = uiScript.tabGroup.selectedTab;
                rememberedDefaultIndex = uiScript.tabGroup.defaultTabIndex;
            }

            // Only open UI if it wasn't already open
            if (!wasMenuOpen)
            {
                uiScript.ToggleUI();
                yield return null; // Wait for UI to fully open
            }

            // Load save data while UI is open
            if (SaveManager.instance != null)
            {
                SaveManager.instance.RefreshAndLoad();
                yield return null; // Wait for data to load
            }

            // Restore tab state if we have a remembered tab
            if (uiScript.tabGroup != null)
            {
                if (rememberedSelectedTab != null)
                {
                    uiScript.tabGroup.OnTabSelected(rememberedSelectedTab);
                }
                else if (uiScript.tabGroup.tabButtons.Count > 0)
                {
                    int safeIndex = Mathf.Clamp(rememberedDefaultIndex, 0, uiScript.tabGroup.tabButtons.Count - 1);
                    uiScript.tabGroup.OnTabSelected(uiScript.tabGroup.tabButtons[safeIndex]);
                }
                yield return null; // Wait for tab restoration
            }

            // Only close UI if we opened it (and it wasn't originally open)
            if (!wasMenuOpen)
            {
                uiScript.ToggleUI();
                yield return null; // Wait for UI to close
            }
        }
        else
        {
            // Fallback without UI
            yield return new WaitForSeconds(0.1f);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.RefreshAndLoad();
            }
        }
    }

    private IEnumerator EnableWaypointAfterDelay(Object_Waypoint waypoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        waypoint.SetCanBeTriggered(true);
    }

    private Object_Waypoint GetWaypoint(Respawn_Type type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);
        foreach (var waypoint in waypoints)
        {
            if (waypoint.GetWaypointType() == type)
                return waypoint;
        }
        return null;
    }
}