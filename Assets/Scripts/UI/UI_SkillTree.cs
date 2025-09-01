using UnityEngine;

// スキルツリーUIを管理するクラス
public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private UI_SkillTooltip skillTooltip; // スキルの詳細ツールチップUI
    [SerializeField] private int skillPoints;             // プレイヤーの残りスキルポイント
    [SerializeField] private UI_TreeConnectHandler[] parentNodes; // 接続ハンドラー配列

    public Player_SkillManager skillManager { get; private set; } // プレイヤーのスキル管理クラス参照

    private void Awake()
    {
        // シーン内からPlayer_SkillManagerを検索して取得
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    // コンテキストメニューから全スキルをリセット
    [ContextMenu("Refund all skills")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund(); // 各ノードのスキルを返却
    }

    // スキルツールチップを取得
    public UI_SkillTooltip SkillTooltip => skillTooltip;

    // スキルポイントが足りるか判定
    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;

    // スキルポイントを消費
    public void RemoveSkillPoint(int cost) => skillPoints -= cost;

    // スキルポイントを追加
    public void AddSkillPoints(int points) => skillPoints += points;

    private void Start()
    {
        // スキルツリーの接続状態を更新
        UpdateAllConnections();
    }

    // 全ての親ノードの接続を更新
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }
}
