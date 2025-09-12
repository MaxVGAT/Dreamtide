using UnityEngine;

public class UI : MonoBehaviour
{

    [System.Serializable]
    private class UIContext
    {
        public GameObject panel;
        public bool isVisible;
        public bool isInsideTrigger;
    }

    public enum NPCType { Storage, Craft, Merchant }

    [SerializeField] private GameObject tabMenuRoot;
    [SerializeField] private UIContext storage = new UIContext();
    [SerializeField] private UIContext craft = new UIContext();
    [SerializeField] private UIContext merchant = new UIContext();

    #region UI Components
    public UI_SkillTree skillTree { get; private set; }
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_StatTooltip statTooltip { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_TabGroup tabGroup { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    #endregion

    private bool menuEnabled;

    private void Awake()
    {
        tabMenuRoot.SetActive(false);
        tabGroup = GetComponentInChildren<UI_TabGroup>(true);

        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();

        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);

        storageUI.storageRoot = storage.panel;

        if (storage.panel != null)
            storage.panel.SetActive(false);

        if (craft.panel != null)
            craft.panel.SetActive(false);

        if (merchant.panel != null)
            merchant.panel.SetActive(false);
    }

    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;

        if (tabMenuRoot)   // null check + destroyed object check
            tabMenuRoot.SetActive(menuEnabled);

        if (!menuEnabled && skillTree != null)
            skillTree.SkillTooltip.ShowToolTip(false, skillTree.SkillTooltip.GetComponent<RectTransform>());

        itemTooltip?.ShowToolTip(false, null, null);

        if (!menuEnabled)
        {
            // Only hide panels if the player is not inside their trigger
            if (!storage.isInsideTrigger)
                ToggleNPCType(storage, false);

            if (!craft.isInsideTrigger)
                ToggleNPCType(craft, false);

            if (!merchant.isInsideTrigger)
                ToggleNPCType(merchant, false);
        }
    }

    private void OpenMenuIfClosed()
    {
        if (!menuEnabled)
        {
            menuEnabled = true;
            tabMenuRoot.SetActive(true);

            if (tabGroup != null && tabGroup.tabButtons.Count > 0)
                tabGroup.OnTabSelected(tabGroup.tabButtons[0]);
        }
    }

    public bool IsMenuOpen() => menuEnabled;

    // Shops Toggle ---
    private void ToggleNPCType(UIContext ctx, bool show)
    {
        ctx.isVisible = show && ctx.isInsideTrigger;
        if (ctx.panel != null)
            ctx.panel.SetActive(ctx.isVisible);
    }

    private void SetInsideTrigger(UIContext ctx, bool inside)
    {
        ctx.isInsideTrigger = inside;
        if(!inside) ToggleNPCType(ctx, false);
    }

    // Storage ---
    public void OpenInventoryWithStorage()
    {
        OpenMenuIfClosed();
        ToggleNPCType(storage, true);
    }

    public void SetInsideShopTrigger(bool inside) => SetInsideTrigger(storage, inside);
    public bool IsInsideShopTrigger() => storage.isInsideTrigger;
    public bool IsStorageVisible() => storage.isVisible;
    public void ShowStorageInInventory(bool show) => ToggleNPCType(storage, show);

    // Craft ---

    public void OpenInventoryWithCraft()
    {
        if (IsCraftVisible())
            return;

        OpenMenuIfClosed();
        ToggleNPCType(craft, true);
    }

    public void SetInsideCraftTrigger(bool inside) => SetInsideTrigger(craft, inside);
    public bool IsInsideCraftTrigger() => craft.isInsideTrigger;
    public bool IsCraftVisible() => craft.isVisible;
    public void ShowCraftInInventory(bool show) => ToggleNPCType(craft, show);

    // Merchant Shop ---

    public void OpenInventoryWithMerchant()
    {
        OpenMenuIfClosed();
        ToggleNPCType(merchant, true);
    }

    public void SetInsideMerchantTrigger(bool inside) => SetInsideTrigger(merchant, inside);
    public bool IsInsideMerchantTrigger() => merchant.isInsideTrigger;
    public bool IsMerchantVisible() => merchant.isVisible;
    public void ShowMerchantInInventory(bool show) => ToggleNPCType(merchant, show);
    
}