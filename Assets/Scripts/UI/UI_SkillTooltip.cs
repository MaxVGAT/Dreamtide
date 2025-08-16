using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private Color exempleColor;
    private string lockedSkillText = "別の決断により、このスキルは封印されました。";
    private string unlockedSkillText = "このスキルはすでに解放済みです。";

    private Coroutine textEffectCo;

    protected override void Awake()
    {
        base.Awake();

        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect) // Override from Tooltip Script
    {
        base.ShowToolTip(show, targetRect);
    }

    
    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node) // Overload ShowToolTip with Skill SO
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        if (textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
            textEffectCo = null;
        }

        skillName.text = node.skillData.skillName;
        skillDescription.text = node.skillData.skillDescription;

        if (node.isUnlocked)
            skillRequirements.text = $"<color={metConditionHex}>{unlockedSkillText}</color>";
        else if(node.isLocked)
            skillRequirements.text = $"<color={notMetConditionHex}>{lockedSkillText}</color>";
        else
            skillRequirements.text = GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);
    }

    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(LockedBlinkEffectCo(skillRequirements, 0.2f, 3));
    }

    public void UnlockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(UnlockedBlinkEffectCo(skillRequirements, 0.2f, 3));
    }

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

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes) 
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("解放条件：");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;
        string costText = $"- {skillCost} スキルポイント";
        string finalCostText = GetColoredText(costColor, costText);

        sb.AppendLine(finalCostText);

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            string nodeText = $"- {node.skillData.skillName}";
            string finalNodeText = GetColoredText(nodeColor, nodeText);

            sb.AppendLine(finalNodeText);
        }

        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine(); //Spacing
        sb.AppendLine($"<color={importantInfoHex}>封印： </color>");

        foreach(var node in conflictNodes)
        {
            sb.AppendLine($"<color={importantInfoHex}>- {node.skillData.skillName} </color>");
        }

        return sb.ToString();
    }

    

    
}
