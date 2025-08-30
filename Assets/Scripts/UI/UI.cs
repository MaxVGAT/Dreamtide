using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot;
    [SerializeField] private UI_SkillTree skillTree;
    public UI_ItemTooltip itemTooltip;

    private bool menuEnabled;

    private void Awake()
    {
        tabMenuRoot.SetActive(false);

        if (skillTree == null)
            skillTree = FindAnyObjectByType<UI_SkillTree>();

        if (itemTooltip == null)
            itemTooltip = FindAnyObjectByType<UI_ItemTooltip>();
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
