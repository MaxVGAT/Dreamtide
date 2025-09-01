using UnityEngine;
using UnityEngine.UIElements;

// プレイヤー基本攻撃状態
public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer; // 攻撃による移動時間のカウント
    private float lastTimeAttacked;    // 最後に攻撃した時間

    private bool comboAttackQueued;    // 次のコンボ攻撃が予約されたか
    private const int FirstComboIndex = 1;
    private int comboIndex = 1;        // 現在のコンボ番号
    private int comboLimit = 3;        // 最大コンボ数
    private int attackDirection;       // 攻撃方向

    public Player_BasicAttackState(Entity_Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
            comboLimit = player.attackVelocity.Length; // コンボ数を攻撃速度配列に合わせる
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndexIfNeeded(); // コンボリセット判定
        SyncAttackSpeed();          // 攻撃速度アニメ同期

        attackDirection = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDirection;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity(); // 攻撃時のスライド
    }

    public override void Update()
    {
        base.Update();
        HandleAttackSliding(); // 攻撃中の移動処理

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack(); // コンボ予約

        if (triggerCalled)
            HandleStateExit(); // 攻撃終了後の状態遷移
    }

    // 攻撃による移動の処理
    private void HandleAttackSliding()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;               // コンボ番号進行
        lastTimeAttacked = Time.time;
    }

    // 攻撃状態終了時の処理
    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay(); // 次のコンボへ
        }
        else
            stateMachine.ChangeState(player.idleState); // アイドルへ
    }

    // 次の攻撃を予約
    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    // 攻撃中にプレイヤーを前進させる
    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDirection, attackVelocity.y);
    }

    // 一定時間経過でコンボリセット
    private void ResetComboIndexIfNeeded()
    {
        if (comboIndex > comboLimit || Time.time > lastTimeAttacked + player.comboAttackWindow)
            comboIndex = FirstComboIndex;
    }
}
