using UnityEngine;

public class Skill_Dash : Skill_Base
{
    // ダッシュ開始時に発動するエフェクト・スキル処理
    public void OnStartEffect()
    {
        // 開始時にクローンを生成するアップグレード判定
        if (Unlocked(Skill_UpgradeType.Dash_CloneOnStart) || Unlocked(Skill_UpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        // 開始時にタイムシャードを生成するアップグレード判定
        if (Unlocked(Skill_UpgradeType.Dash_ShardOnStart) || Unlocked(Skill_UpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    // ダッシュ終了時に発動するエフェクト・スキル処理
    public void OnEndEffect()
    {
        // 終了時にクローンを生成するアップグレード判定
        if (Unlocked(Skill_UpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        // 終了時にタイムシャードを生成するアップグレード判定
        if (Unlocked(Skill_UpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    // タイムシャード生成（スキルマネージャ経由）
    private void CreateShard()
    {
        skillManager.shard.CreateRawShard();
    }

    // クローン生成（スキルマネージャ経由）
    private void CreateClone()
    {
        skillManager.timeEcho.CreateTimeEcho();
    }
}
