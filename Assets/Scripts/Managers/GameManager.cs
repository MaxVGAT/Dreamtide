using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Respawn_Type lastRespawnType;

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
        StartCoroutine(ChangeSceneCo(sceneName));
    }

    private IEnumerator ChangeSceneCo(string sceneName)
    {
        // Wait a frame to ensure any pending operations complete
        yield return null;

        // Save the game BEFORE changing scenes to preserve current progress
        Debug.Log("Saving game before scene change...");
        SaveManager.instance?.SaveGame();
        Debug.Log("Save completed!");

        // Add a small delay to ensure save completes
        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(TeleportAfterSceneLoad());
    }

    private IEnumerator TeleportAfterSceneLoad()
    {
        // Get waypoint and disable it immediately
        Object_Waypoint targetWaypoint = GetWaypoint(lastRespawnType);
        if (targetWaypoint != null)
            targetWaypoint.SetCanBeTriggered(false);

        // Wait until the player exists
        while (Entity_Player.instance == null)
            yield return null;

        // Wait until the skill manager (or other dependencies) exists
        var skillManager = Entity_Player.instance.GetComponent<Player_SkillManager>();
        while (skillManager == null)
        {
            yield return null;
            skillManager = Entity_Player.instance.GetComponent<Player_SkillManager>();
        }

        // Now safe to load saved data
        SaveManager.instance?.RefreshAndLoad();

        yield return null; // one frame to let data settle

        // Teleport player
        if (targetWaypoint != null)
            Entity_Player.instance.TeleportPlayer(targetWaypoint.GetRespawnPosition());
    }



    private Vector3 GetWaypointPosition(Respawn_Type type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var points in waypoints)
        {
            if (points.GetWaypointType() == type)
                return points.GetRespawnPosition();
        }
        return Vector3.zero;
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