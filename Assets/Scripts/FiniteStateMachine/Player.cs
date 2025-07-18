using UnityEngine;

public class Player : MonoBehaviour
{

    public Animator anim { get; private set; }

    public Rigidbody2D rb { get; private set; }

    private StateMachine stateMachine;

    public PlayerInputSet inputActions { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState {  get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }

    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5f;
    public float inAirMoveMultiplier = 0.8f;
    private bool facingRight = true;
    public Vector2 moveInput { get; private set; }

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    public bool isGrounded { get; private set; }
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
        inputActions = new PlayerInputSet();

        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += context => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            FlipMethod();
        else if (xVelocity < 0 && facingRight == true)
            FlipMethod();
    }

    private void FlipMethod()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
    }
}
