using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }

    private IEnumerator ChangeSceneCo(string sceneName, Respawn_Type respawnType)
    {
        // Fade Effect

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(0.2f);

        Vector3 position = GetWaypointPosition(respawnType);

        if (position != Vector3.zero)
            Entity_Player.instance.TeleportPlayer(position);
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
}