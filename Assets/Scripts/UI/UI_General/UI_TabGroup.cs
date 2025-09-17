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

    // Define the expected tab order by name
    private string[] expectedTabOrder = new string[]
    {
        "Tab_CharacterProfile",  // Index 0
        "Tab_Inventory",         // Index 1
        "Tab_Skills",           // Index 2
        "Tab_Settings"              // Index 3
    };

    void Awake()
    {
        ui = GetComponentInParent<UI>();
        defaultTabIndex = 0;
    }

    void Start()
    {
        tabButtons = new List<UI_TabButton>(); // Clear old references

        foreach (var btn in GetComponentsInChildren<UI_TabButton>())
            Subscribe(btn);

        StartCoroutine(InitializeDefaultTab());
    }

    private System.Collections.IEnumerator InitializeDefaultTab()
    {
        // Sort tab buttons by expected order to fix scene transition issues
        SortTabButtonsByExpectedOrder();

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

    private void SortTabButtonsByExpectedOrder()
    {
        if (tabButtons == null || tabButtons.Count == 0) return;

        var sortedTabs = new List<UI_TabButton>();

        // Add tabs in the expected order
        foreach (string expectedName in expectedTabOrder)
        {
            var tab = tabButtons.Find(t => t.name.Contains(expectedName) || t.name == expectedName);
            if (tab != null)
                sortedTabs.Add(tab);
        }

        // Add any remaining tabs that weren't in the expected list
        foreach (var tab in tabButtons)
        {
            if (!sortedTabs.Contains(tab))
                sortedTabs.Add(tab);
        }

        tabButtons = sortedTabs;
    }

    public void Subscribe(UI_TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<UI_TabButton>();

        if (!tabButtons.Contains(button))
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

        int index = button.tabIndex >= 0 ? button.tabIndex : tabButtons.IndexOf(button);

        if (ui != null)
        {
            if (index == 1)
            {
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

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            bool shouldActivate = (i == index);
            objectsToSwap[i].SetActive(shouldActivate);

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
