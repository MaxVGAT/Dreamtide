using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot;
    [SerializeField] private UI_SkillTree skillTree;
    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;

    private bool menuEnabled;

    private void Awake()
    {
        tabMenuRoot.SetActive(false);

        skillTree = GetComponentInChildren<UI_SkillTree>();
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();
    }

    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;

        if (tabMenuRoot != null)
            tabMenuRoot.SetActive(!menuEnabled);

        if (itemTooltip != null)
            itemTooltip.ShowToolTip(false, null, null);
        else
            return;
    }
}
