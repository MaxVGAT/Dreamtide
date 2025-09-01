using UnityEngine;

// アイテムや装備のレアリティを表す列挙型
public enum Item_Rarity
{
    Common,      // 一般的なアイテム。入手しやすく、性能は控えめ
    Uncommon,    // やや珍しいアイテム。少し強力な効果を持つ
    Rare,        // 希少なアイテム。性能や特殊効果が目立つ
    Epic,        // 非常に希少で強力なアイテム。入手は困難
    Legendary    // 伝説級のアイテム。特別な性能や唯一性を持つ
}