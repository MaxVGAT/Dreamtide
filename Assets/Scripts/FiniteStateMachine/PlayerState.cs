using UnityEditor.Tilemaps;
using UnityEngine;

public abstract class PlayerState : EntityState
{

    protected Entity_Player player;
    protected PlayerInputSet input;

    public PlayerState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
        {
            if (input.Player.Dash.WasPressedThisFrame() && CanDash())
                stateMachine.ChangeState(player.dashState);
        }

    }

private bool CanDash()
    {
        if (player.isWallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
    }

}
