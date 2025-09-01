using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard; // 現在操作中のシャード
    private Entity_Health playerHealth;     // プレイヤーの体力情報

    [SerializeField] private GameObject shardPrefab; // シャードのプレハブ
    [SerializeField] private float detonateTime = 2f; // 通常シャードの自動爆発時間

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 6; // 移動するシャードの速度

    [Header("MultiCast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;   // マルチキャスト時の最大使用回数
    [SerializeField] private int currentCharges;   // 現在の残りチャージ
    [SerializeField] private bool isRecharging;    // チャージ回復中かどうか

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f; // テレポートシャードの存在時間

    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent; // ヘルスリワインド用保存体力

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges; // 初期チャージセット
        playerHealth = GetComponentInParent<Entity_Health>();
    }

    // 通常シャード生成
    public void CreateShard()
    {
        float detonationTime = GetDetonateTime();

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this);

        // テレポート系シャードなら爆発時にクールダウンを強制
        if (Unlocked(Skill_UpgradeType.Shard_Teleport) || Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            currentShard.OnExplode += ForceCooldown;
    }

    // 指定ターゲットに向かうシャード生成（移動可能か指定）
    public void CreateRawShard(Transform target = null, bool shardsCanMove = false)
    {
        bool canMove = shardsCanMove != false ? shardsCanMove :
            Unlocked(Skill_UpgradeType.Shard_MoveToEnemy) || Unlocked(Skill_UpgradeType.Shard_MultiCast);

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed, target);
    }

    public void CreateDomainShard(Transform target)
    {
        // ドメインキャスト用（未実装）
    }

    public override void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        // スキルタイプごとの挙動
        if (Unlocked(Skill_UpgradeType.Shard))
            HandleShardRegular();

        if (Unlocked(Skill_UpgradeType.Shard_MoveToEnemy))
            HandleShardMoving();

        if (Unlocked(Skill_UpgradeType.Shard_MultiCast))
            HandleShardMulticast();

        if (Unlocked(Skill_UpgradeType.Shard_Teleport))
            HandleShardTeleport();

        if (Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            HandleShardHealthRewind();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCooldown();
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        SetSkillOnCooldown();
    }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0)
            return;

        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
        currentCharges--;

        if (!isRecharging)
            StartCoroutine(ShardRechargeCo());
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
            CreateShard();
        else
        {
            SwapPlayerAndShard(); // プレイヤーとシャード位置を入れ替え
            SetSkillOnCooldown();
        }
    }

    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthPercent(); // 現在体力を保存
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercent); // 保存した体力に戻す
            SetSkillOnCooldown();
        }
    }

    // プレイヤーとシャードの位置を入れ替える
    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        currentShard.Explode();

        player.TeleportPlayer(shardPosition);
    }

    // マルチキャスト用チャージ回復
    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;

        while (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }

        isRecharging = false;
    }

    // シャードの自動爆発時間を取得
    public float GetDetonateTime()
    {
        if (Unlocked(Skill_UpgradeType.Shard_Teleport) || Unlocked(Skill_UpgradeType.Shard_TeleportHPRewind))
            return shardExistDuration;

        return detonateTime;
    }

    // テレポート系シャード用：爆発時にスキルクールダウンを強制
    private void ForceCooldown()
    {
        if (!OnCooldown())
        {
            SetSkillOnCooldown();
            currentShard.OnExplode -= ForceCooldown;
        }
    }
}
