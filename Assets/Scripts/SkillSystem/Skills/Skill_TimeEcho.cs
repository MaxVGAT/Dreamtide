using UnityEngine;

// タイムエコースキルの管理クラス
public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab; // タイムエコーとして生成するオブジェクト
    [SerializeField] private float timeEchoDuration;    // タイムエコーの存続時間

    [Header("Attack upgrades")]
    [SerializeField] private int maxAttacks = 3;        // 最大攻撃回数（マルチアタック用）
    [SerializeField] private float duplicateChance = 0.3f; // 攻撃が複製される確率

    [Header("Heal Wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = 0.3f; // ヒールウィスプが回復する割合
    [SerializeField] private float cooldownReducedInSeconds;  // ウィスプ使用時のクールダウン短縮時間

    // ウィスプ効果の回復割合を取得
    public float GetPercentOfDamageHealed()
    {
        if (!ShouldBeWisp())
            return 0;

        return damagePercentHealed;
    }

    // ウィスプ効果時のクールダウン短縮量を取得
    public float GetCooldownReduceInSeconds()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_CooldownWisp)
            return 0;

        return cooldownReducedInSeconds;
    }

    // ネガティブ効果を除去できるか判定
    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    // ウィスプとして機能するか判定
    public bool ShouldBeWisp()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_HealWisp
            || upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    // 攻撃複製の確率を取得
    public float GetDuplicateChance()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 0;

        return duplicateChance;
    }

    // 最大攻撃回数を取得
    public int GetMaxAttacks()
    {
        if (upgradeType == Skill_UpgradeType.TimeEcho_SingleAttack
            || upgradeType == Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;

        if (upgradeType == Skill_UpgradeType.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;
    }

    // タイムエコーの存続時間を取得
    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    // スキル使用処理
    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        // タイムエコーを生成
        Vector3 exactPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        CreateTimeEcho(exactPosition);
        SetSkillOnCooldown();
    }

    // タイムエコーの生成
    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position; // 指定がなければ自身の位置に生成

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this); // スキルデータをセット
    }
}
