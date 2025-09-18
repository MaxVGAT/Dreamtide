using UnityEngine;

// リスポーンタイプ定義
public enum Respawn_Type
{
    Enter,       // 入場時リスポーン
    Exit,        // 退場時リスポーン
    Portal,      // ポータル経由リスポーン
    NonSpecific  // 特定なし
}
