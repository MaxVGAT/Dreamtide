using UnityEngine;
using TMPro;

public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    public override void ShowToolTip(bool show, RectTransform targetRect) // Override from Tooltip Script
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, Skill_DataSO skillData) // Overload ShowToolTip with Skill SO
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        skillName.text = skillData.skillName;
        skillDescription.text = skillData.skillDescription;
        skillRequirements.text = "         \n\n " +
            " - " + skillData.cost + " スキルポイント";
    }
}
