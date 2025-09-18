using UnityEngine;

// ダメージを受ける対象のインターフェース
public interface IDamageable
{
    // ダメージ処理メソッド
    // damage: 物理ダメージ量
    // elementalDamage: 属性ダメージ量
    // element: 属性タイプ
    // damageDealer: ダメージを与えたオブジェクトのTransform
    // return: ダメージが適用されたかどうか
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer);
}
