using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler
{
    public Inventory_Item itemInSlot { get; private set; }
    private Inventory_Player inventory;

    [Header("UI Slot Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackSize;

    // double click
    private float lastClickTime;
    private const float DoubleClickThreshold = 0.3f; // seconds

    private void Awake()
    {
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.TryEquipItem(itemInSlot);
            lastClickTime = 0;
        }
        else
            lastClickTime = Time.time;
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            itemStackSize.text = " ";
            itemIcon.color = Color.clear;
            return;
        }

        Color color = Color.white; color.a = 0.9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSize.text = item.stackSize > 1 ? item.stackSize.ToString() : " ";
        itemStackSize.color = item.stackSize < itemInSlot.itemData.maxStackSize ? itemStackSize.color : Color.yellow;
    }


}