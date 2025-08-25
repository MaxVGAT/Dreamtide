using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    // フェードアウトアニメーションの時間
    private float deathAnimDuration = 2f;

    public EnemyDeadState(Entity_Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 敵が死亡したときにコルーチンを開始
        enemy.StartCoroutine(WaitAndDespawn());
    }

    private IEnumerator WaitAndDespawn()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        // 死亡アニメーション終了後、フェードアウトのコルーチンを開始
        enemy.DespawnOnDeath(2f);
    }
}
