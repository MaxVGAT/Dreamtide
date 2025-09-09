using System.Collections.Generic;
using UnityEditor.TerrainTools;
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

    void Start()
    {
        ui = GetComponentInParent<UI>(); // Get UI component
        if (tabButtons != null && tabButtons.Count > 0)
        {
            if (defaultTabIndex >= 0 && defaultTabIndex < tabButtons.Count)
            {
                OnTabSelected(tabButtons[defaultTabIndex]);
            }
            else
            {
                OnTabSelected(tabButtons[0]);
            }
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
        int index = (button.TryGetComponent<UI_TabButton>(out var tabButton) && tabButton.tabIndex >= 0)
            ? tabButton.tabIndex
            : tabButtons.IndexOf(button);

        if (ui != null)
        {
            // Inventory tab is index 0
            if (index == 0)
            {
                // Let UIContext handle trigger checks internally
                ui.ShowStorageInInventory(true);
                ui.ShowCraftInInventory(true);
            }
            else
            {
                ui.ShowStorageInInventory(false);
                ui.ShowCraftInInventory(false);
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