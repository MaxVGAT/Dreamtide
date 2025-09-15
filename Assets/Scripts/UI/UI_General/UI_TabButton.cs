using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// タブボタン用クラス
[RequireComponent(typeof(Image))]
public class UI_TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UI_TabGroup tabGroup; // 属するタブグループ
    public Image background;     // ボタン背景

    [Header("Tab Settings")]
    public int tabIndex = -1;    // このタブのインデックス（-1なら自動）

    private void Start()
    {
        // Image コンポーネントを取得
        background = GetComponent<Image>();
        // タブグループにこのボタンを登録
        tabGroup.Subscribe(this);
    }

    // クリック時に選択
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    // マウスオーバー時
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    // マウスが離れた時
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }


}