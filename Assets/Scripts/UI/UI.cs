using UnityEngine;

// ゲームのUI全体を管理するクラス
public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot; // タブメニュー全体のルートオブジェクト
    [SerializeField] private UI_SkillTree skillTree; // スキルツリーUI
    public UI_ItemTooltip itemTooltip;               // アイテムツールチップUI
    public UI_StatTooltip statTooltip;               // ステータスツールチップUI

    private bool menuEnabled; // メニューの表示状態

    private void Awake()
    {
        // メニューを初期非表示に設定
        tabMenuRoot.SetActive(false);

        // 子オブジェクトからUIコンポーネントを取得
        skillTree = GetComponentInChildren<UI_SkillTree>();
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>();
        statTooltip = GetComponentInChildren<UI_StatTooltip>();
    }

    // UIの表示・非表示を切り替える
    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;

        if (tabMenuRoot != null)
            tabMenuRoot.SetActive(!menuEnabled); // メニューの表示状態を反転

        if (itemTooltip != null)
            itemTooltip.ShowToolTip(false, null, null); // ツールチップを非表示に
        else
            return;
    }
}
