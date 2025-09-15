using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Respawn_Type lastRespawnType;
    private bool isTeleporting = false; // Add this flag

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

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        lastRespawnType = respawnType;
        SceneManager.sceneLoaded += OnSceneLoaded; // Re-subscribe only when needed
        StartCoroutine(ChangeSceneCo(sceneName));
    }

    private IEnumerator ChangeSceneCo(string sceneName)
    {
        yield return null;
        SaveManager.instance?.SaveGame();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded - unsubscribing immediately");
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe right away

        StartCoroutine(TeleportAfterSceneLoad());
    }

    private IEnumerator TeleportAfterSceneLoad()
    {
        isTeleporting = true; // Set flag

        // Get waypoint and disable it immediately
        Object_Waypoint targetWaypoint = GetWaypoint(lastRespawnType);
        if (targetWaypoint != null)
            targetWaypoint.SetCanBeTriggered(false);

        // Wait until the player exists
        while (Entity_Player.instance == null)
            yield return null;

        // Wait until the skill manager exists
        var skillManager = Entity_Player.instance.GetComponent<Player_SkillManager>();
        while (skillManager == null)
        {
            yield return null;
            skillManager = Entity_Player.instance.GetComponent<Player_SkillManager>();
        }

        // Give one frame for everything to settle
        yield return null;

        if (targetWaypoint != null)
            Entity_Player.instance.TeleportPlayer(targetWaypoint.GetRespawnPosition());


        var uiScript = FindAnyObjectByType<UI>(); // Replace with your actual UI script name
        if (uiScript != null)
        {
            Debug.Log("Temporarily opening UI for initialization...");
            uiScript.ToggleUI(); // Open UI

            yield return null; // Let everything initialize
            yield return null;

            // Load save data while UI is open
            Debug.Log("Loading save data...");
            SaveManager.instance?.RefreshAndLoad();

            // Close UI again
            uiScript.ToggleUI(); // Close UI
            Debug.Log("UI closed after save load");
        }
        else
        {
            // Fallback
            yield return new WaitForSeconds(0.5f);
            SaveManager.instance?.RefreshAndLoad();
        }
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