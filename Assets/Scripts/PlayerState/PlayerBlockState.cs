using UnityEngine;

public class PlayerBlockState : PlayerState
{

    private float blockDuration = 0.5f;
    private float blockTimer;

    public PlayerBlockState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        blockTimer = blockDuration;
    }

    public override void Update()
    {
        base.Update();

        blockTimer -= Time.deltaTime;

        player.SetVelocity(0, rb.linearVelocity.y);

        if (blockTimer <= 0 || !input.Player.Block.IsPressed())
            stateMachine.ChangeState(player.idleState);
    }

}
