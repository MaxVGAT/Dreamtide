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

        int index;
        if (button.TryGetComponent<UI_TabButton>(out var tabButton) && tabButton.tabIndex >= 0)
        {
            index = tabButton.tabIndex;
        }
        else
        {
            index = tabButtons.IndexOf(button);
        }

        // Hide storage unless selecting inventory tab (index 0) and inside shop trigger
        if (ui != null && index != 0) // Assuming inventory tab is at index 0
        {
            ui.ShowStorageInInventory(false);
            ui.ShowCraftInInventory(false);
        }
        else if (ui != null && index == 0)
        {
            ui.ShowStorageInInventory(ui.IsInsideShopTrigger()); // Show storage only if in trigger
            ui.ShowCraftInInventory(ui.IsInsideCraftTrigger());
        }

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            bool shouldActivate = (i == index);
            objectsToSwap[i].SetActive(shouldActivate);

            if (i == index && objectsToSwap[i].TryGetComponent<UI_Inventory>(out var inventory))
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