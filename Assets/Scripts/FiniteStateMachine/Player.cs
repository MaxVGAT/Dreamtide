using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine stateMachine;

    private PlayerIdleState idleState;

    private void Awake()
    {
        stateMachine = new StateMachine();

        idleState = new PlayerIdleState(stateMachine, "idle");
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }
}
