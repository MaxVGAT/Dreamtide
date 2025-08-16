using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Entity_Player player;

    [Header("Camera details")]
    public float moveSpeed;
    public float lookAheadDistance = 2.5f;
    public float lookAheadSpeed = 0.5f;
    public float verticalOffset = 2f;
    private float lookOffset;
    public float maxVertOffset = 5f;
    private bool isFalling;

    private Vector3 targetPoint = Vector3.zero;

    private void Start()
    {
        targetPoint = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        if (player.isGrounded)
            targetPoint.y = player.transform.position.y;

        float targetY = player.transform.position.y + verticalOffset;

        if (transform.position.y - (player.transform.position.y + verticalOffset) > maxVertOffset)
            isFalling = true;

        if(isFalling)
        {
            targetY = player.transform.position.y;
            if (player.isGrounded)
                isFalling = false;
        }

        float targetLookOffset = lookAheadDistance * player.facingDirection;
        lookOffset = Mathf.Lerp(lookOffset, targetLookOffset, lookAheadSpeed * Time.deltaTime);

        targetPoint = new Vector3(player.transform.position.x + lookOffset, targetY, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }
}
