using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

// スキルツリーのツールチップUIを管理するクラス
public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI skillName;         // スキル名表示
    [SerializeField] private TextMeshProUGUI skillDescription;  // スキル説明
    [SerializeField] private TextMeshProUGUI skillCooldown;     // クールダウン表示
    [SerializeField] private TextMeshProUGUI skillRequirements; // 解放条件表示

    [Space]
    [SerializeField] private string metConditionHex;       // 条件達成時の色コード
    [SerializeField] private string notMetConditionHex;    // 条件未達成時の色コード
    [SerializeField] private string importantInfoHex;      // 重要情報の色コード
    [SerializeField] private Color exempleColor;          // サンプルカラー
    private string lockedSkillText = "別の決断により、このスキルは封印されました。";
    private string unlockedSkillText = "このスキルはすでに解放済みです。";

    private Coroutine textEffectCo; // 点滅エフェクト用コルーチン

    protected override void Awake()
    {
        base.Awake();
    }

    // 基本のツールチップ表示（オーバーライド）
    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    // スキルデータを指定してツールチップ表示
    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node, bool hasEnoughPoints)
    {
        base.ShowToolTip(show, targetRect);

        if (!show) return;

        // 前回の点滅コルーチンを停止
        if (textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
            textEffectCo = null;
        }

        skillName.text = node.skillData.skillName;
        skillDescription.text = node.skillData.skillDescription;
        skillCooldown.text = "クールダウン：" + node.skillData.upgradeData.cooldown + "秒";

        if (node.isUnlocked)
            skillRequirements.text = GetColoredText(metConditionHex, unlockedSkillText);
        else if (node.isLocked)
            skillRequirements.text = GetColoredText(notMetConditionHex, lockedSkillText);
        else
            skillRequirements.text = GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes, hasEnoughPoints);
    }

    // 封印スキル点滅エフェクト
    public void LockedSkillEffect()
    {
        if (textEffectCo != null) StopCoroutine(textEffectCo);
        textEffectCo = StartCoroutine(LockedBlinkEffectCo(skillRequirements, 0.2f, 3));
    }

    // 解放スキル点滅エフェクト
    public void UnlockedSkillEffect()
    {
        if (textEffectCo != null) StopCoroutine(textEffectCo);
        textEffectCo = StartCoroutine(UnlockedBlinkEffectCo(skillRequirements, 0.2f, 3));
    }

    // 封印スキル点滅処理
    private IEnumerator LockedBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(importantInfoHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(notMetConditionHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    // 解放スキル点滅処理
    private IEnumerator UnlockedBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(importantInfoHex, unlockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(metConditionHex, unlockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    // スキル解放条件の文字列生成
    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes, bool hasEnoughPoints)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("解放条件：");

        string costColor = hasEnoughPoints ? metConditionHex : notMetConditionHex;
        sb.AppendLine(GetColoredText(costColor, $"- {skillCost} スキルポイント"));

        foreach (var node in neededNodes)
        {
            if (node == null) continue;
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine(GetColoredText(nodeColor, $"- {node.skillData.skillName}"));
        }

        if (conflictNodes.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine(GetColoredText(importantInfoHex, "封印："));

            foreach (var node in conflictNodes)
            {
                if (node == null) continue;
                sb.AppendLine(GetColoredText(importantInfoHex, $"- {node.skillData.skillName}"));
            }
        }

        return sb.ToString();
    }
}
