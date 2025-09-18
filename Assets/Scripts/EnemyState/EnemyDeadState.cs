using System.Collections;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    // 死亡アニメーションの再生時間
    private float deathAnimDuration = 2f;

    public EnemyDeadState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 物理判定を無効化して攻撃を受けないようにする
        Collider2D col2D = enemy.GetComponent<Collider2D>();
        if (col2D != null) col2D.enabled = false;

        Rigidbody2D rb2D = enemy.GetComponent<Rigidbody2D>();
        if (rb2D != null) rb2D.simulated = false; // ノックバックや移動を停止

        // アニメーション再生後に消去するコルーチンを開始
        enemy.StartCoroutine(WaitAndDespawn());
    }

    private IEnumerator WaitAndDespawn()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        // 死亡後の消去処理（エフェクトやオブジェクト削除）
        enemy.DespawnOnDeath(2f);
    }
}
