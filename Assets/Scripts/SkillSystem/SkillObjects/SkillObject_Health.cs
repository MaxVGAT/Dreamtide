using UnityEngine;

/// <summary>
/// スキルオブジェクト専用のヘルス管理クラス
/// </summary>
public class SkillObject_Health : Entity_Health
{
    /// <summary>
    /// 死亡時の処理をオーバーライド
    /// </summary>
    protected override void Die()
    {
        // 親スキルオブジェクト(TimeEcho)の死亡処理を呼び出す
        SkillObject_TimeEcho timeEcho = GetComponent<SkillObject_TimeEcho>();
        timeEcho.HandleDeath();
    }
}
