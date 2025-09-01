using UnityEngine;

// プレイヤーのジャンプ攻撃状態
public class Player_JumpAttackState : PlayerState
{
    private bool touchedGround;

    public Player_JumpAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        touchedGround = false;

        // ジャンプ攻撃の初速度を設定
        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDirection, player.jumpAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // 地面に触れた瞬間、攻撃アニメーションを発動
        if (player.isGrounded && touchedGround == false)
        {
            touchedGround = true;
            anim.SetTrigger("jumpAttackTrigger");
            player.SetVelocity(0, rb.linearVelocity.y); // 横速度は止める
        }

        // 攻撃トリガーが呼ばれ、地面に着地したら待機状態に戻す
        if (triggerCalled && player.isGrounded)
            stateMachine.ChangeState(player.idleState);
    }
}
