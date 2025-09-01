using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab; // プレハブ化されたタイムエコー
    [SerializeField] private float timeEchoDuration;    // タイムエコーの存在時間

    [Header("Attack upgrades")]
    [SerializeField] private int maxAttacks = 3;        // 最大攻撃回数（マルチアタック用）
    [SerializeField] private float duplicateChance = 0.3f; // 攻撃複製の確率

    [Header("Heal Wisp Upgrades")]
    [SerializeField] private float damagePercentHealed = 0.3f; // 回復量（受けたダメージの割合）
    [SerializeField] private float cooldownReducedInSeconds;  // クールダウン短縮量

    // Wisp形態の場合に回復割合を返す
    public float GetPercentOfDamageHealed()
    {
        if (!ShouldBeWisp())
            return 0;

        return damagePercentHealed;
    }

    // Wispアップグレードが有効な場合、クールダウン短縮量を返す
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

    // 現在のアップグレードが回復/クレンズWispか判定
    public bool ShouldBeWisp()
    {
        return upgradeType == Skill_UpgradeType.TimeEcho_HealWisp
            || upgradeType == Skill_UpgradeType.TimeEcho_CleanseWisp;
    }

    // 複製攻撃の確率を返す
    public float GetDuplicateChance()
    {
        if (upgradeType != Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 0;

        return duplicateChance;
    }

    // 最大攻撃回数を返す
    public int GetMaxAttacks()
    {
        if (upgradeType == Skill_UpgradeType.TimeEcho_SingleAttack
            || upgradeType == Skill_UpgradeType.TimeEcho_ChanceToDuplicate)
            return 1;

        if (upgradeType == Skill_UpgradeType.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;
    }

    // タイムエコーの存在時間を返す
    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    // スキル使用処理
    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        // エコーを作成する位置を決定（現在位置）
        Vector3 exactPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        CreateTimeEcho(exactPosition);
    }

    // タイムエコーを生成
    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position; // 指定がなければ現在位置

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this); // セットアップ
    }
}
