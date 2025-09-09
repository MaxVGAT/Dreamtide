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

    public UI_SkillTree skillTree { get; private set; }
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_StatTooltip statTooltip { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_TabGroup tabGroup { get; private set; }
    public UI_Craft craftUI { get; private set; }

    private bool menuEnabled;

    private void Awake()
    {
        tabMenuRoot.SetActive(false);

        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        tabGroup = GetComponentInChildren<UI_TabGroup>(true);

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
        tabMenuRoot.SetActive(menuEnabled);

        itemTooltip?.ShowToolTip(false, null, null);

        if (!menuEnabled)
        {
            ToggleNPCType(storage, false);
            ToggleNPCType(craft, false);
            ToggleNPCType(merchant, false);

            storage.isInsideTrigger = false;
            craft.isInsideTrigger = false;
            merchant.isInsideTrigger = false;
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
        OpenMenuIfClosed();
        ToggleNPCType(craft, true);
    }

    public void SetInsideCraftTrigger(bool inside) => SetInsideTrigger(craft, inside);
    public bool IsInsideCraftTrigger() => craft.isInsideTrigger;
    public bool IsCraftVisible() => craft.isVisible;
    public void ShowCraftInInventory(bool show) => ToggleNPCType(craft, show);

    // Merchant Shop ---

    public void OpenInventoryWithShop()
    {
        OpenMenuIfClosed();
        ToggleNPCType(merchant, true);
    }

    public void SetInsideMerchantTrigger(bool inside) => SetInsideTrigger(merchant, inside);
    public bool IsInsideMerchantTrigger() => merchant.isInsideTrigger;
    public bool IsMerchantVisible() => merchant.isVisible;
    public void ShowMerchantInInventory(bool show) => ToggleNPCType(merchant, show);
    
}