using System.Collections.Generic;
using System.Linq;
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

    private UI ui;
    private bool isInitialized = false;

    // Define the expected tab order by name (adjust these to match your actual tab names)
    private string[] expectedTabOrder = new string[]
    {
        "Tab_CharacterProfile",        // Index 0
        "Tab_Inventory",    // Index 1 (Character Profile)
        "Tab_Skills",        // Index 2
        "Tab_Settings",      // Index 3 (Settings)
        // Add more tab names as needed
    };

    void Awake()
    {
        ui = GetComponentInParent<UI>();
        defaultTabIndex = 0;
        isInitialized = false;
    }

    void Start()
    {
        StartCoroutine(InitializeDefaultTab());
    }

    private System.Collections.IEnumerator InitializeDefaultTab()
    {
        // Sort tab buttons by expected order based on names
        SortTabButtonsByExpectedOrder();

        // Ensure all objects start inactive
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

        isInitialized = true;
    }

    private void SortTabButtonsByExpectedOrder()
    {
        if (tabButtons == null || tabButtons.Count == 0) return;

        // Create a new sorted list based on expected order
        var sortedTabs = new List<UI_TabButton>();

        // First, add tabs in the expected order
        foreach (string expectedName in expectedTabOrder)
        {
            var tab = tabButtons.Find(t => t.name.Contains(expectedName) || t.name == expectedName);
            if (tab != null)
            {
                sortedTabs.Add(tab);
                Debug.Log($"Added tab in order: {tab.name} at index {sortedTabs.Count - 1}");
            }
        }

        // Then add any remaining tabs that weren't in the expected list
        foreach (var tab in tabButtons)
        {
            if (!sortedTabs.Contains(tab))
            {
                sortedTabs.Add(tab);
                Debug.Log($"Added remaining tab: {tab.name} at index {sortedTabs.Count - 1}");
            }
        }

        // Replace the original list
        tabButtons = sortedTabs;

        // Debug final order
        Debug.Log("Final tab button order:");
        for (int i = 0; i < tabButtons.Count; i++)
        {
            Debug.Log($"Index {i}: {tabButtons[i].name}");
        }
    }

    public void Subscribe(UI_TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<UI_TabButton>();

        if (!tabButtons.Contains(button))
        {
            tabButtons.Add(button);
        }
    }

    public void OnTabEnter(UI_TabButton button)
    {
        if (!isInitialized) return;

        ResetTabs();
        if (selectedTab == null || button != selectedTab)
            button.background.sprite = tabHover;
    }

    public void OnTabExit(UI_TabButton button)
    {
        if (!isInitialized) return;

        ResetTabs();
    }

    public void OnTabSelected(UI_TabButton button)
    {
        // Add this debug logging
        Debug.Log($"=== TAB SELECTION DEBUG ===");
        Debug.Log($"Selected button: {button.name}");
        Debug.Log($"Button's tabIndex field: {button.tabIndex}");
        Debug.Log($"Button's position in list: {tabButtons.IndexOf(button)}");
        Debug.Log($"Total tabs in list: {tabButtons.Count}");
        for (int i = 0; i < tabButtons.Count; i++)
        {
            Debug.Log($"  List[{i}]: {tabButtons[i].name} (tabIndex: {tabButtons[i].tabIndex})");
        }

        Debug.Log($"========================");
        if (button == null) return;

        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;

        // Use list index since we've sorted the list correctly
        int index = tabButtons.IndexOf(button);

        Debug.Log($"Tab selected: {button.name} at index {index}");

        if (ui != null)
        {
            // Inventory tab is index 1
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

        // Swap objects
        if (index >= 0 && index < objectsToSwap.Count)
        {
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
    }

    public void ResetTabs()
    {
        if (tabButtons == null) return;

        foreach (UI_TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }

    public void ForceReset()
    {
        isInitialized = false;
        selectedTab = null;
        StartCoroutine(InitializeDefaultTab());
    }
}