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

    private bool playerIsInside = false;

    private void Start()
    {
        playerIsInside = false;
        canBeTriggered = true;
    }
    
    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToScene;
        if (waypointType == Respawn_Type.Enter)
            connectedWaypoint = Respawn_Type.Exit;
        if (waypointType == Respawn_Type.Exit)
            connectedWaypoint = Respawn_Type.Enter;
    }

    private void OnEnable()
    {
        // Always reset state on scene load
        playerIsInside = false;

        // Waypoint stays disabled until the player leaves its collider
        // (prevents instant re-trigger if spawn overlaps)
        canBeTriggered = false;
    }

    public Respawn_Type GetWaypointType() => waypointType;

    public void SetCanBeTriggered(bool canBeTriggered)
    {
        this.canBeTriggered = canBeTriggered;
    }

    public Vector3 GetRespawnPosition()
    {
        return respawnPoint == null ? transform.position : respawnPoint.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeTriggered || playerIsInside) return;

        if (collision.GetComponent<Entity_Player>() == null) return;

        playerIsInside = true;
        canBeTriggered = false;

        GameManager.instance.ChangeScene(transferToScene, connectedWaypoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity_Player>() != null)
        {
            playerIsInside = false;
            canBeTriggered = true; // re-armed only after exit
        }
    }
}