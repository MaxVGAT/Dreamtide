using UnityEngine;

public class UI_EquipSlot : UI_ItemSlot
{
    public Item_Type slotType;

    private void OnValidate()
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();
    }
}
