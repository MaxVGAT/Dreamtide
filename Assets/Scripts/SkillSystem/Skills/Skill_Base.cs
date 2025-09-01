using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Entity_Player player { get; private set; }             // 所属プレイヤー
    public Player_SkillManager skillManager { get; private set; } // スキル管理コンポーネント

    public DamageScaleData damageScaleData { get; private set; }  // スキルのダメージ倍率・属性情報

    [Header("General details")]
    [SerializeField] protected Skill_Type skillType;             // スキルの種類
    [SerializeField] protected Skill_UpgradeType upgradeType;    // スキルのアップグレード状態
    [SerializeField] protected float cooldown;                  // クールダウン時間
    private float lastTimeUsed;                                   // 最後に使用した時間

    // 初期化処理
    protected virtual void Awake()
    {
        player = GetComponentInParent<Entity_Player>();         // 親からプレイヤー参照
        skillManager = GetComponentInParent<Player_SkillManager>(); // スキルマネージャ参照
        lastTimeUsed = lastTimeUsed - cooldown;                // 初回使用可能状態にする
        damageScaleData = new DamageScaleData();               // ダメージ情報初期化
    }

    // スキル使用を試みる（オーバーライド用）
    public virtual void TryUseSkill()
    {
        // 子クラスで処理
    }

    // スキルアップグレードを適用
    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;                     // アップグレードタイプ設定
        cooldown = upgrade.cooldown;                           // クールダウン設定
        damageScaleData = upgrade.damageScaleData;            // ダメージ情報設定
        ResetCooldown();                                       // クールダウンリセット
    }

    // スキルが使用可能か判定
    public virtual bool CanUseSkill()
    {
        if (upgradeType == Skill_UpgradeType.None)
            return false;                                     // スキル未取得なら不可

        if (OnCooldown())
            return false;                                     // クールダウン中は不可

        // TODO: 解放条件やマナ量判定など追加可能

        return true;
    }

    // 特定のアップグレードを持っているか
    protected bool Unlocked(Skill_UpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    // クールダウン中か判定
    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    // 使用時にクールダウンを設定
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;

    // クールダウンを短縮
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;

    // クールダウンをリセットして即使用可能にする
    public void ResetCooldown() => lastTimeUsed = Time.time - cooldown;
}
