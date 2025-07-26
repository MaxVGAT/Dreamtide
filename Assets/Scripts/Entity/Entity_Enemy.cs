using UnityEngine;

public class Entity_Enemy : Entity
{

    public Transform visual;
    public Vector3 visualOffset;

    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;

    [Header("Movement details")]
    public float moveSpeed = 1.4f;
    public float idleTime = 2f;
    [Range(0, 2)] public float moveAnimSpeedMultiplier = 1f;

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;

    private void OnAnimatorMove()
    {
        if (visual != null)
        {
            visual.position = transform.position + new Vector3(visualOffset.x * facingDirection, visualOffset.y, visualOffset.z);
        }
    }

    public RaycastHit2D PlayerDetection()
    {
        return Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
    }

    public void SnapVisualToRoot(int facingDirection)
    {
        Vector3 offset = visual.localPosition;
        offset.x *= facingDirection; // Flip offset if facing left
        transform.position += offset;
        visual.localPosition = Vector3.zero;
    }

}
