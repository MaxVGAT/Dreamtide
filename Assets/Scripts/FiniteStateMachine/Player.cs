using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine stateMachine;
    private PlayerInputSet inputActions;

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState {  get; private set; }

    public Vector2 moveInput { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine();
        inputActions = new PlayerInputSet();

        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
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
        stateMachine.UpdateActiveState();
    }
}
