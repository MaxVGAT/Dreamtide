using UnityEngine;

// ダメージを受けることができるオブジェクト用インターフェース
public interface IDamageable
{
    // ダメージを与えたときに呼ばれる関数
    // damage: 物理ダメージ量
    // elementalDamage: 属性ダメージ量
    // element: 属性タイプ
    // damageDealer: ダメージを与えたオブジェクトのTransform
    // 戻り値: ダメージが適用されたかどうか
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer);
}
