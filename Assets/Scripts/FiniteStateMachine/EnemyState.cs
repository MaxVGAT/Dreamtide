using UnityEngine;

public class EnemyState : EntityState
{
    protected Entity_Enemy enemy;

    // コンストラクタ：特定の敵にこのステートを関連付け、必要な参照を設定
    public EnemyState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        // 敵のコンポーネントをキャッシュして簡単にアクセスできるようにする
        rb = enemy.rb;
        anim = enemy.anim;
        stats = enemy.stats;
    }

    // 毎フレームアニメーションパラメータを更新
    // ステートマシンによって呼び出され、アニメーションと物理挙動を同期
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        // バトルアニメーションの速度を基本移動速度に対して計算
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;

        // アニメーションの速度倍率を適用
        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);

        // 水平方向の速度をアニメーションに渡す（ブレンドツリーなど用）
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }
}
