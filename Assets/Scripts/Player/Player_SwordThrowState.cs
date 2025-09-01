using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;

    public Player_SwordThrowState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 予測用ドットを有効化
        skillManager.swordThrow.EnableDots(true);

        // メインカメラ取得
        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    public override void Update()
    {
        base.Update();

        // マウス方向の単位ベクトル取得
        Vector2 dirToMouse = DirectionToMouse();

        // 移動停止
        player.SetVelocity(0, rb.linearVelocity.y);
        // プレイヤーの向き更新
        player.HandleFlip(dirToMouse.x);
        // 軌道予測表示
        skillManager.swordThrow.PredictTrajectory(dirToMouse);

        // 攻撃入力時
        if (input.Player.Attack.WasPressedThisFrame())
        {
            anim.SetBool("swordThrowPerformed", true);

            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(dirToMouse);
        }

        // 攻撃ボタン離した or アニメーションイベント発火時
        if (input.Player.RangeAttack.WasReleasedThisFrame() || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        // アニメーションと予測ドットをリセット
        anim.SetBool("swordThrowPerformed", false);
        skillManager.swordThrow.EnableDots(false);
    }

    // マウス方向への単位ベクトルを返す
    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);

        Vector2 direction = worldMousePosition - playerPosition;

        return direction.normalized;
    }
}
