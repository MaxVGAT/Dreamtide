using UnityEngine;

// スキルのアップグレード種類を表す列挙型
public enum Skill_UpgradeType
{
    None, // アップグレードなし

    // --- ダッシュツリー ---
    Dash, // ダッシュでダメージ回避
    Dash_CloneOnStart, // ダッシュ開始時にクローンを作る
    Dash_CloneOnStartAndArrival, // ダッシュ開始と到達時にクローンを作る
    Dash_ShardOnStart, // ダッシュ開始時にタイムシャードを作る
    Dash_ShardOnStartAndArrival, // ダッシュ開始と到達時にタイムシャードを作る

    // --- シャードツリー ---
    Shard, // シャードは敵接触または遅延後に爆発
    Shard_MoveToEnemy, // シャードが最も近い敵に向かって移動
    Shard_MultiCast, // シャードのチャージを複数持て、連続で発動可能
    Shard_Teleport, // 作成した最後のシャードと位置を入れ替える
    Shard_TeleportHPRewind, // シャードと入れ替えた際、作成時のHP%に戻る

    // --- タイムエコーツリー ---
    TimeEcho, // プレイヤーのクローンを作る。クローンは敵のダメージを受ける
    TimeEcho_SingleAttack, // クローンは単発攻撃可能
    TimeEcho_MultiAttack, // クローンは複数回攻撃可能
    TimeEcho_ChanceToDuplicate, // クローンが攻撃時に別のクローンを作る確率
    TimeEcho_HealWisp, // クローン死亡時、プレイヤーに向かうウィスプを生成して回復
    TimeEcho_CleanseWisp, // ウィスプがプレイヤーのデバフも解除するようになる
    TimeEcho_CooldownWisp, // ウィスプが全スキルのクールダウンをN秒短縮

    // --- ソードスローツリー ---
    SwordThrow, // 剣を投げて遠距離の敵にダメージ
    SwordThrow_Spin, // 剣が回転してダメージを与える
    SwordThrow_Pierce, // 剣が複数の敵を貫通
    SwordThrow_Bounce, // 剣が敵間で跳ね返る

    // --- ドメインエクスパンションツリー ---
    Domain_Slow, // 敵が凍る範囲を作るがプレイヤーは自由に戦える
    Domain_Echo, // 移動不可だがタイムエコーを連続で使用可能
    Domain_Shard // 移動不可だがタイムシャードを連続で使用可能
}
