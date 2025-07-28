using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{

    public Animator anim { get; private set; }

    public Rigidbody2D rb { get; private set; }

    protected StateMachine stateMachine;

    public int facingDirection { get; private set; } = 1;
    private bool facingRight = true;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform upperWallCheck;
    [SerializeField] private Transform lowerWallCheck;

    public bool isWallDetected { get; private set; }
    public bool isGrounded { get; private set; }

    private bool isKnocked;
    private Coroutine knockbackCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if(knockbackCo !=null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {

        if (isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            FlipMethod();
        else if (xVelocity < 0 && facingRight == true)
            FlipMethod();
    }

    public void FlipMethod()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (lowerWallCheck != null)
        {
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround)
                          && Physics2D.Raycast(lowerWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        }
        else
            isWallDetected = Physics2D.Raycast(upperWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(upperWallCheck.position, upperWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));

        if(lowerWallCheck != null)
        Gizmos.DrawLine(lowerWallCheck.position, lowerWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }
}
