using UnityEditor;
using UnityEngine;

// ƒvƒŒƒCƒ„[‚Ì€–Só‘Ô
public class Player_DeadState : PlayerState
{
    public Player_DeadState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // “ü—Í‚ğ–³Œø‰»
        input.Disable();

        // •¨—‹““®‚ğ’â~
        rb.simulated = false;
    }
}
