using System.Linq;
using TMPro;
using UnityEngine;

// プレイヤー用スキルツリーUIの管理クラス
public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private UI_SkillTooltip skillTooltip; // スキルのツールチップUI
    [SerializeField] private TextMeshProUGUI skillPointsText; // 残りスキルポイント表示
    [SerializeField] private UI_TreeConnectHandler[] parentNodes; // スキルツリーの接続ハンドラー
    private UI_TreeNode[] allTreeNodes; // ツリーノードの全リスト

    public Player_SkillManager skillManager { get; private set; } // プレイヤーのスキル管理

    private void Awake()
    {
        // シーン内のPlayer_SkillManagerを取得
        skillManager = FindAnyObjectByType<Player_SkillManager>();
        skillManager.OnSkillPointsChanged += UpdateSkillPointsText;

        // 初期スキルポイント表示更新
        UpdateSkillPointsText(skillManager.skillPoints);

        // 全ノード取得
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>();
    }

    private void Start()
    {
        // スキルツリーの接続線を初期表示
        UpdateAllConnections();
    }

    // スキルポイント表示更新
    private void UpdateSkillPointsText(int points)
    {
        if (skillPointsText != null)
            skillPointsText.text = skillManager.skillPoints.ToString();
    }

    // すべてのスキルを返却（リセット）する
    [ContextMenu("Refund all skills")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund(); // 個別ノードをリセット
    }

    // スキルツールチップ取得
    public UI_SkillTooltip SkillTooltip => skillTooltip;

    // スキルポイントが足りているか判定
    public bool EnoughSkillPoints(int cost) => skillManager.skillPoints >= cost;

    // スキルポイント消費
    public void RemoveSkillPoint(int cost)
    {
        if (skillManager.SpendSkillPoints(cost))
            UpdateSkillPointsText(skillManager.skillPoints);
    }

    // スキルポイント追加
    public void AddSkillPoints(int points)
    {
        skillManager.AddSkillPoints(points);
        UpdateSkillPointsText(skillManager.skillPoints);
    }

    // スキルツリーの接続更新
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }

    // データロード
    public void LoadData(GameData data)
    {
        if (skillManager == null)
            skillManager = FindAnyObjectByType<Player_SkillManager>();

        if (skillManager == null)
        {
            Debug.Log("SkillManager not found - skipping skill tree load");
            return;
        }

        // スキルポイント反映
        skillManager.skillPoints = data.skillPoints;
        UpdateSkillPointsText(skillManager.skillPoints);

        // ノードのアンロック処理
        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
            {
                bool canUnlock = true;

                // 必要な親ノードが全てアンロックされているかチェック
                foreach (var neededNode in node.neededNodes)
                {
                    if (!data.skillTreeUI.TryGetValue(neededNode.skillData.skillName, out bool parentNodeUnlocked) || !parentNodeUnlocked)
                    {
                        canUnlock = false;
                        break;
                    }
                }

                // 競合ノードがアンロックされていないかチェック
                if (canUnlock)
                {
                    foreach (var conflictNode in node.conflictNodes)
                    {
                        if (data.skillTreeUI.TryGetValue(conflictNode.skillData.skillName, out bool conflictUnlocked) && conflictUnlocked)
                        {
                            canUnlock = false;
                            break;
                        }
                    }
                }

                // 条件を満たしていればノードをアンロック
                if (canUnlock)
                    node.UnlockWithSaveData();
                else
                    data.skillTreeUI[skillName] = false;
            }
        }

        // スキルアップグレード反映
        foreach (var skill in skillManager.allSkills)
        {
            if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out Skill_UpgradeType upgradeType))
            {
                var upgradeNode = allTreeNodes.FirstOrDefault(node => node.skillData.upgradeData.upgradeType == upgradeType);

                if (upgradeNode != null && upgradeNode.isUnlocked)
                    skill.SetSkillUpgrade(upgradeNode.skillData);
                else
                    data.skillUpgrades.Remove(skill.GetSkillType());
            }
        }
    }

    // データセーブ
    public void SaveData(ref GameData data)
    {
        data.skillPoints = skillManager.skillPoints;

        data.skillTreeUI.Clear();
        data.skillUpgrades.Clear();

        // ノードのアンロック状態を保存
        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.skillName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        // スキルアップグレード状態を保存
        foreach (var skill in skillManager.allSkills)
        {
            data.skillUpgrades[skill.GetSkillType()] = skill.GetUpgrade();
        }
    }
}
