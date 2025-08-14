using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillTooltip skillToolTip;

    private void Awake()
    {
        skillToolTip = GetComponentInChildren<UI_SkillTooltip>();
    }
}
