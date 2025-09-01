using UnityEngine;

// Time Echo系スキルオブジェクトのアニメーション用トリガー
public class SkillObject_AnimationTriggers : MonoBehaviour
{
    private SkillObject_TimeEcho timeEcho;

    private void Awake()
    {
        // 親オブジェクトからTimeEchoスキルコンポーネントを取得
        timeEcho = GetComponentInParent<SkillObject_TimeEcho>();
    }

    // アニメーションイベント用: 攻撃を発動
    private void AttackTrigger()
    {
        timeEcho.PerformAttack();
    }

    // アニメーションイベント用: 攻撃の終了判定
    private void TryTerminate(int currentAttackIndex)
    {
        if (currentAttackIndex == timeEcho.maxAttacks)
            timeEcho.HandleDeath();
    }
}
