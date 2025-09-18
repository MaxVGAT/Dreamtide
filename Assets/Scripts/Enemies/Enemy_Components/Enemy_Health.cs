using UnityEngine;

public class Enemy_Health : Entity_Health // 敵用の体力管理クラス
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>(); // 同じGameObjectのEntity_Enemy参照

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canTakeDamage == false)
            return false; // ダメージ不可なら処理せず false を返す

        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        // プレイヤーから攻撃された場合、戦闘状態に移行
        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        // ダメージ適用後に死亡判定
        if (isDead)
        {
            Die(); // 死亡処理（VFX、ドロップなど）
        }

        // ✅ 基底クラスでダメージを受けた場合は true を返す
        return wasHit;
    }
}
