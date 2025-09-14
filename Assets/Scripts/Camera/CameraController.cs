using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Entity_Player player;

    [Header("Camera movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookAheadDistance = 2.5f;
    [SerializeField] private float lookAheadSpeed = 0.5f;
    [SerializeField] private float verticalOffset = 2f;
    [SerializeField] private float maxVertOffset = 5f;

    [Header("Bounds (optional)")]
    [SerializeField] private bool useBounds = false;   // enable this if you want limits in this scene
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Vector3 targetPoint;
    private float lookOffset;
    private bool isFalling;

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Entity_Player>();
        }

        targetPoint = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z
        );
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Handle vertical follow
        float targetY = player.transform.position.y + verticalOffset;
        if (transform.position.y - targetY > maxVertOffset)
            isFalling = true;

        if (isFalling)
        {
            targetY = player.transform.position.y;
            if (player.isGrounded)
                isFalling = false;
        }

        // Look ahead horizontally
        float targetLookOffset = lookAheadDistance * player.facingDirection;
        lookOffset = Mathf.Lerp(lookOffset, targetLookOffset, lookAheadSpeed * Time.deltaTime);

        // Target camera position
        targetPoint = new Vector3(
            player.transform.position.x + lookOffset,
            targetY,
            transform.position.z
        );

        // Apply bounds if enabled
        if (useBounds)
        {
            targetPoint.x = Mathf.Clamp(targetPoint.x, minBounds.x, maxBounds.x);
            targetPoint.y = Mathf.Clamp(targetPoint.y, minBounds.y, maxBounds.y);
        }

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }
}
