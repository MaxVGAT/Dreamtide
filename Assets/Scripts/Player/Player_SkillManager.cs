using UnityEngine;

// プレイヤーのスキル管理クラス
// 各スキルへの参照を保持し、クールダウン管理やスキル取得を提供
public class Player_SkillManager : MonoBehaviour
{
    // 各スキルのプロパティ
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_Domain domain { get; private set; }

    private Skill_Base[] allSkills; // 全スキルの配列（共通処理用）

    private void Awake()
    {
        // 子オブジェクトから各スキルコンポーネントを取得
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        domain = GetComponentInChildren<Skill_Domain>();

        // 全スキルを配列として取得（共通処理用）
        allSkills = GetComponentsInChildren<Skill_Base>();
    }

    // 全スキルのクールダウンを減少させる
    public void ReduceAllSkillsBooldownBy(float amount)
    {
        foreach (var skills in allSkills)
            skills.ReduceCooldownBy(amount);
    }

    // スキルタイプからスキルを取得
    public Skill_Base GetSkillByType(Skill_Type type)
    {
        switch (type)
        {
            case Skill_Type.Dash:
                return dash;
            case Skill_Type.TimeShard:
                return shard;
            case Skill_Type.SwordThrow:
                return swordThrow;
            case Skill_Type.TimeEcho:
                return timeEcho;
            case Skill_Type.Domain:
                return domain;
            default:
                Debug.Log("Not implemented");
                return null;
        }
    }
}
