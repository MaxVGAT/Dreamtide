using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_ItemSlot
{
    public Item_Type slotType;

    private void OnValidate()
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;

        if (Time.time - lastClickTime < DoubleClickThreshold)
        {
            inventory.UnequipItem(itemInSlot);
            lastClickTime = 0;
        }
        else
        {
            lastClickTime = Time.time;
            base.OnPointerDown(eventData); // this won't compile as 'base' can't call interface implementation
        }
    }
}