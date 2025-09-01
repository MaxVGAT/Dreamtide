using UnityEngine;

// 地上状態用の基底クラス
public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 落下判定（地上にいない場合）
        if (rb.linearVelocity.y < 0 && player.isGrounded == false)
            stateMachine.ChangeState(player.fallState);

        // 各入力に応じて状態を切り替え
        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if (input.Player.Block.WasPressedThisFrame())
            stateMachine.ChangeState(player.blockState);

        if (input.Player.Counter.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttackState);

        if (input.Player.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill())
            stateMachine.ChangeState(player.swordThrowState);
    }
}
