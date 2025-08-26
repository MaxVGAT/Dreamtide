using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_TabGroup : MonoBehaviour
{
    public List<UI_TabButton> tabButtons;
    public List<GameObject> objectsToSwap;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public UI_TabButton selectedTab;

    public void Subscribe(UI_TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<UI_TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(UI_TabButton button)
    {
        ResetTabs();

        if(selectedTab == null || button != selectedTab)
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

        int index = button.transform.GetSiblingIndex();
        for(int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);

                if (objectsToSwap[i].TryGetComponent<UI_Inventory>(out var inventory))
                {
                    inventory.RefreshInventoryUI();
                }
            }
            else
                objectsToSwap[i].SetActive(false);
        }
    }

    public void ResetTabs()
    {
        foreach(UI_TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }
}
