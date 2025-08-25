using UnityEngine;

public class Enemy_Health : Entity_Health // すべての体力を持つエンティティの基本クラス
{
    private Entity_Enemy enemy => GetComponent<Entity_Enemy>(); // ダメージ処理のためにEntity_Enemyコンポーネントを取得

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        // 敵が攻撃を受けたか確認し、ダメージを適用。その後、可能なら（戦闘状態でなければ）戦闘状態へ移行
        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        // 敵が死亡していれば戦闘状態に入る必要がないので処理を終了
        if (isDead)
            return false;

        // ダメージを与えたのがプレイヤーなら、敵は戦闘状態へ移行を試みる
        if (damageDealer.GetComponent<Entity_Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }
}
