using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
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

    protected override void Awake()
    {
        base.Awake();

        skillTree = GetComponentInParent<UI_SkillTree>();
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

        skillName.text = node.skillData.skillName;
        skillDescription.text = node.skillData.skillDescription;

        if (node.isUnlocked)
            skillRequirements.text = $"<color={metConditionHex}>{unlockedSkillText}</color>";
        else if(node.isLocked)
            skillRequirements.text = $"<color={notMetConditionHex}>{lockedSkillText}</color>";
        else
            skillRequirements.text = GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);


    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes) 
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("解放条件：");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionHex : notMetConditionHex;

        sb.AppendLine($"<color={costColor}>- {skillCost} スキルポイント </color>");

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine($"<color={nodeColor}>- {node.skillData.skillName} </color>");
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
