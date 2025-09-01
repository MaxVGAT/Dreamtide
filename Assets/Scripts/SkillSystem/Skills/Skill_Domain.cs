using UnityEngine;
using System.Collections.Generic;

public class Skill_Domain : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("Domain Details")]
    public float maxDomainSize = 10f; // 最大サイズ
    public float expandSpeed = 10f;   // 拡大速度

    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;     // スロー効果の割合
    [SerializeField] private float slowDownDomainDuration = 5f; // スロー持続時間

    [Header("Shard Cast Upgrade")]
    [SerializeField] private int shardsToCast = 10;             // ドメイン内でキャストするシャード数
    [SerializeField] private float shardCastDomainSlow = 1f;    // シャード用スロー効果
    [SerializeField] private float shardCastDomainDuration = 5f;// シャード用持続時間
    private float spellCastTimer;
    private float spellsPerSecond;

    [Header("Time Echo Cast Upgrade")]
    [SerializeField] private int echoToCast = 8;                // ドメイン内でキャストするタイムエコー数
    [SerializeField] private float echoCastDomainSlow = 1f;    // タイムエコー用スロー効果
    [SerializeField] private float echoCastDomainDuration = 5f;// タイムエコー用持続時間

    private List<Entity_Enemy> trappedTargets = new List<Entity_Enemy>(); // ドメイン内に捕らえた敵リスト
    private Transform currentTarget; // 現在キャスト対象の敵

    // ドメインを生成して初期化
    public void CreateDomain()
    {
        spellsPerSecond = GetSpellsToCast() / GetDomainDuration();
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_Domain>().SetupDomain(this);
    }

    // ドメイン内で定期的にスペルをキャスト
    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;

        if (currentTarget == null)
            currentTarget = FindTargetInDomain(); // 次の対象を取得

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);          // 対象にスペルをキャスト
            spellCastTimer = 1 / spellsPerSecond; // タイマーリセット
            currentTarget = null;
        }
    }

    // 対象に応じてスペルをキャスト
    private void CastSpell(Transform target)
    {
        if (upgradeType == Skill_UpgradeType.Domain_Echo)
        {
            Vector3 offset = Random.value < 0.5f ? new Vector2(2, 0) : new Vector2(-2, 0); // 左右ランダムオフセット
            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if (upgradeType == Skill_UpgradeType.Domain_Shard)
        {
            skillManager.shard.CreateRawShard(target, true);
        }
    }

    // ドメイン内の有効な敵ターゲットをランダムで取得
    private Transform FindTargetInDomain()
    {
        trappedTargets.RemoveAll(target => target == null || target.health.isDead); // 無効な敵を除去

        if (trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);
        return trappedTargets[randomIndex].transform;
    }

    // ドメインの持続時間を取得
    public float GetDomainDuration()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownDomainDuration;
        else if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoCastDomainDuration;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardCastDomainDuration;

        return 0;
    }

    // ドメインのスロー効果を取得
    public float GetSlowPercentage()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Slow)
            return slowDownPercent;
        else if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoCastDomainSlow;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardCastDomainSlow;

        return 0;
    }

    // キャストするスペルの数を取得
    private int GetSpellsToCast()
    {
        if (upgradeType == Skill_UpgradeType.Domain_Echo)
            return echoToCast;
        else if (upgradeType == Skill_UpgradeType.Domain_Shard)
            return shardsToCast;

        return 0;
    }

    // エコーやシャード以外は即時ドメインとして扱う
    public bool InstantDomain()
    {
        return upgradeType != Skill_UpgradeType.Domain_Echo
            && upgradeType != Skill_UpgradeType.Domain_Shard;
    }

    // ドメインに敵を追加
    public void AddTarget(Entity_Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);
    }

    // ドメイン内の敵ターゲットをクリア
    public void ClearTargets()
    {
        foreach (var enemy in trappedTargets)
            enemy.StopSlowDown(); // スロー効果解除

        trappedTargets = new List<Entity_Enemy>();
    }
}
