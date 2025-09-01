using UnityEngine;

// プレイヤーブロック状態
public class Player_BlockState : PlayerState
{
    private float blockDuration = 0.5f; // ブロック持続時間
    private float blockTimer;            // ブロックタイマー

    public Player_BlockState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        blockTimer = blockDuration; // タイマー初期化
    }

    public override void Update()
    {
        base.Update();

        blockTimer -= Time.deltaTime; // タイマー更新

        player.SetVelocity(0, rb.linearVelocity.y); // ブロック中は水平移動停止

        // タイマー終了またはブロック入力解除で状態遷移
        if (blockTimer <= 0 || !input.Player.Block.IsPressed())
            stateMachine.ChangeState(player.idleState);
    }
}
