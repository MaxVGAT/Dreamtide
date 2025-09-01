using UnityEngine;

// 空中状態用の基底クラス
public class PlayerAiredState : PlayerState
{
    public PlayerAiredState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    // 毎フレーム更新
    public override void Update()
    {
        base.Update();

        // 空中での左右移動
        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);

        // 空中攻撃入力でジャンプ攻撃状態へ
        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpAttackState);
    }
}
