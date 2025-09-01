using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// タブグループ用クラス
public class UI_TabGroup : MonoBehaviour
{
    public List<UI_TabButton> tabButtons;     // グループ内のタブボタン
    public List<GameObject> objectsToSwap;    // タブ切替対象のUIオブジェクト

    public Sprite tabIdle;   // 非選択時スプライト
    public Sprite tabHover;  // ホバー時スプライト
    public Sprite tabActive; // 選択時スプライト
    public UI_TabButton selectedTab; // 現在選択されているタブ

    // タブボタンを登録
    public void Subscribe(UI_TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<UI_TabButton>();

        tabButtons.Add(button);
    }

    // マウスオーバー
    public void OnTabEnter(UI_TabButton button)
    {
        ResetTabs();

        if (selectedTab == null || button != selectedTab)
            button.background.sprite = tabHover;
    }

    // マウス離脱
    public void OnTabExit(UI_TabButton button)
    {
        ResetTabs();
    }

    // タブ選択時
    public void OnTabSelected(UI_TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == index);

            // Inventory UIなら更新
            if (i == index && objectsToSwap[i].TryGetComponent<UI_Inventory>(out var inventory))
            {
                inventory.RefreshInventoryUI();
            }
        }
    }

    // タブを初期状態に戻す（非選択スプライト）
    public void ResetTabs()
    {
        foreach (UI_TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }
}
