using System.Collections.Generic;
using UnityEngine;

public class UI_TabGroup : MonoBehaviour
{
    public List<UI_TabButton> tabButtons;
    public List<GameObject> objectsToSwap;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public UI_TabButton selectedTab;
    public int defaultTabIndex = 0;

    private UI ui; // Reference to UI manager

    void Awake()
    {
        ui = GetComponentInParent<UI>();
        defaultTabIndex = 0;
    }

    void Start()
    {
        StartCoroutine(InitializeDefaultTab());
    }

    private System.Collections.IEnumerator InitializeDefaultTab()
    {
        foreach (var obj in objectsToSwap)
            obj.SetActive(false);

        yield return null;

        if (tabButtons != null && tabButtons.Count > 0)
        {
            int safeIndex = (defaultTabIndex >= 0 && defaultTabIndex < tabButtons.Count)
                ? defaultTabIndex
                : 0;

            OnTabSelected(tabButtons[safeIndex]);
        }
    }

    public void Subscribe(UI_TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<UI_TabButton>();

        tabButtons.Add(button);
    }

    public void OnTabEnter(UI_TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.sprite = tabHover;
    }

    public void OnTabExit(UI_TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UI_TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;

        // Determine tab index
        int index = button.tabIndex >= 0 ? button.tabIndex : tabButtons.IndexOf(button);

        if (ui != null)
        {
            // Inventory tab is index 1
            if (index == 1)
            {
                // Let UIContext handle trigger checks internally
                ui.ShowStorageInInventory(true);
                ui.ShowCraftInInventory(true);
                ui.ShowMerchantInInventory(true);
            }
            else
            {
                ui.ShowStorageInInventory(false);
                ui.ShowCraftInInventory(false);
                ui.ShowMerchantInInventory(false);
            }
        }

        // Swap objects
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            bool shouldActivate = (i == index);
            objectsToSwap[i].SetActive(shouldActivate);

            // Refresh inventory UI if needed
            if (shouldActivate && objectsToSwap[i].TryGetComponent<UI_Inventory>(out var inventory))
            {
                inventory.RefreshInventoryUI();
            }
        }
    }

    public void ResetTabs()
    {
        foreach (UI_TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }
}