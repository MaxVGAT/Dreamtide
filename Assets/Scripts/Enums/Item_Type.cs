using UnityEngine;

// アイテムの種類を表す列挙型
public enum Item_Type
{
    Material,   // 素材アイテム。クラフトや強化用に使用

    Weapon,     // 武器アイテム。攻撃力や特殊効果を持つ

    // --- 防具部位
    Helmet,     // 頭部装備
    Shoulders,  // 肩装備
    Chest,      // 胴装備
    Pants,      // 脚装備
    Cape,       // マント装備
    Bracers,    // 腕装備
    Gloves,     // 手装備
    Boots,      // 足装備

    // --- アクセサリー類（指輪、護符、書物など）
    Ring,       // 指輪
    Rune        // ルーン、特殊効果付与用
}