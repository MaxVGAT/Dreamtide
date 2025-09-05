using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot;
    [SerializeField] private GameObject storagePanel;

    public UI_SkillTree skillTree { get; private set; }
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_StatTooltip statTooltip { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_TabGroup tabGroup { get; private set; }

    private bool menuEnabled;
    private bool isStorageVisible = false;
    private bool isInsideShopTrigger = false; // New flag to track shop trigger state

    private void Awake()
    {
        tabMenuRoot.SetActive(false);
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();
        storageUI = GetComponentInChildren<UI_Storage>(true);
        tabGroup = GetComponentInChildren<UI_TabGroup>(true);

        if (storagePanel != null)
            storagePanel.SetActive(false);
    }

    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;
        if (tabMenuRoot != null)
            tabMenuRoot.SetActive(menuEnabled);

        if (itemTooltip != null)
            itemTooltip.ShowToolTip(false, null, null);

        if (!menuEnabled)
        {
            ShowStorageInInventory(false);
            isInsideShopTrigger = false; // Reset trigger state when closing menu
        }
    }

    public void ShowStorageInInventory(bool show)
    {
        isStorageVisible = show && isInsideShopTrigger; // Only show if inside shop trigger
        if (storagePanel != null)
        {
            storagePanel.SetActive(isStorageVisible);
        }
    }

    public void OpenInventoryWithStorage()
    {
        if (!menuEnabled)
        {
            menuEnabled = true;
            tabMenuRoot.SetActive(true);
        }

        // Assuming inventory tab is at index 0
        if (tabGroup != null && tabGroup.tabButtons.Count > 0)
        {
            tabGroup.OnTabSelected(tabGroup.tabButtons[0]);
        }

        // Show storage only if inside shop trigger
        ShowStorageInInventory(true);
    }

    // New method to set shop trigger state
    public void SetInsideShopTrigger(bool inside)
    {
        isInsideShopTrigger = inside;
        if (!inside)
        {
            ShowStorageInInventory(false); // Hide storage when exiting trigger
        }
    }

    public bool IsInsideShopTrigger()
    {
        return isInsideShopTrigger;
    }

    public bool IsStorageVisible()
    {
        return isStorageVisible;
    }

    public bool IsMenuOpen()
    {
        return menuEnabled;
    }
}