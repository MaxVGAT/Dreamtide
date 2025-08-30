using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; }
    protected Inventory_Player inventory;
    protected UI ui;
    protected RectTransform rect;

    [Header("UI Slot Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackSize;

    // double click
    protected float lastClickTime;
    protected const float DoubleClickThreshold = 0.3f; // seconds

    protected void Awake()
    {
        ui = GetComponentInParent<UI>();
        inventory = FindAnyObjectByType<Inventory_Player>();
        rect = GetComponent<RectTransform>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || itemInSlot.itemData.itemType == Item_Type.Material)
            return;

        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.TryEquipItem(itemInSlot);
            lastClickTime = 0;
        }
        else
            lastClickTime = Time.time;

        if(itemInSlot == null)
            ui.itemTooltip.ShowToolTip(false, null);
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            if (itemStackSize != null) itemStackSize.text = " ";
            if (itemIcon != null) itemIcon.color = Color.clear;
            return;
        }

        if (itemIcon != null)
        {
            Color color = Color.white;
            color.a = 0.9f;
            itemIcon.color = color;
            itemIcon.sprite = itemInSlot.itemData.itemIcon;
        }

        if (itemStackSize != null)
        {
            itemStackSize.text = item.stackSize > 1 ? item.stackSize.ToString() : " ";
            itemStackSize.color = item.stackSize < itemInSlot.itemData.maxStackSize ? itemStackSize.color : Color.yellow;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        ui.itemTooltip.ShowToolTip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.ShowToolTip(false, null);
    }
}