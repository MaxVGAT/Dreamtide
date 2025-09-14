using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{

    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private Respawn_Type waypointType;
    [SerializeField] private Respawn_Type connectedWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered = true;

    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToScene;

        if (waypointType == Respawn_Type.Enter)
            connectedWaypoint = Respawn_Type.Exit;

        if (waypointType == Respawn_Type.Exit)
            connectedWaypoint = Respawn_Type.Enter;
    }

    public Respawn_Type GetWaypointType() => waypointType;

    public void SetCanBeTriggered(bool canBeTriggered) => this.canBeTriggered = canBeTriggered;

    public Vector3 GetRespawnPosition()
    {
        return respawnPoint == null ? transform.position : respawnPoint.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;

        SaveManager.instance.SaveGame();

        GameManager.instance.ChangeScene(transferToScene, connectedWaypoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }
}
