using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// インベントリの1スロットUIを管理するクラス
// マテリアル以外のアイテムはクリックで装備可能
public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; } // このスロットに入っているアイテム
    protected Inventory_Player inventory; // プレイヤーのインベントリ
    protected UI ui; // UI全体への参照
    protected RectTransform rect; // スロットのRectTransform

    [Header("UI Slot Setup")]
    [SerializeField] private Image itemIcon; // アイテムアイコン
    [SerializeField] private TextMeshProUGUI itemStackSize; // アイテムのスタック数表示

    // ダブルクリック用
    protected float lastClickTime;
    protected const float DoubleClickThreshold = 0.3f; // ダブルクリックと認識する秒数

    protected void Awake()
    {
        ui = GetComponentInParent<UI>(); // UIコンポーネント取得
        inventory = FindAnyObjectByType<Inventory_Player>(); // プレイヤーインベントリ取得
        rect = GetComponent<RectTransform>(); // RectTransform取得
    }

    // スロットクリック時
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // スロットが空、またはマテリアルは処理しない
        if (itemInSlot == null || itemInSlot.itemData.itemType == Item_Type.Material)
            return;

        // ダブルクリック判定
        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.TryEquipItem(itemInSlot); // アイテム装備を試みる
            lastClickTime = 0;
        }
        else
            lastClickTime = Time.time;

        if (itemInSlot == null)
            ui.itemTooltip.ShowToolTip(false, null);
    }

    // スロットの内容を更新
    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            // スロットが空ならアイコンとスタック数を非表示
            if (itemStackSize != null) itemStackSize.text = " ";
            if (itemIcon != null) itemIcon.color = Color.clear;
            return;
        }

        // アイコンを更新
        if (itemIcon != null)
        {
            Color color = Color.white;
            color.a = 0.9f; // 少し透明
            itemIcon.color = color;
            itemIcon.sprite = itemInSlot.itemData.itemIcon;
        }

        // スタック数を更新
        if (itemStackSize != null)
        {
            itemStackSize.text = item.stackSize > 1 ? item.stackSize.ToString() : " ";
            itemStackSize.color = item.stackSize < itemInSlot.itemData.maxStackSize ? itemStackSize.color : Color.yellow;
        }
    }

    // マウスオーバーでツールチップ表示
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        ui.itemTooltip.ShowToolTip(true, rect, itemInSlot);
    }

    // マウスが離れたらツールチップ非表示
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.ShowToolTip(false, null);
    }
}
