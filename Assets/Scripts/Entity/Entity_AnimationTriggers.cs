using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();          // 親オブジェクトからEntityコンポーネントを取得
        entityCombat = GetComponentInParent<Entity_Combat>(); // 親オブジェクトからEntity_Combatコンポーネントを取得
    }

    // 現在のアニメーションステートのトリガーを呼び出す（存在する場合）
    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    // アニメーションイベント用。攻撃判定を特定のフレームで実行し、アニメーションと当たり判定を同期させる
    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }
}
