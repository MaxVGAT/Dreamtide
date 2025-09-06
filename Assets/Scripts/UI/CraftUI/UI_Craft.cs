using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButtons;

    private void Awake()
    {
        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>();

        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        foreach (var button in craftListButtons)
            button.SetCraftSlot(craftSlots);
    }
}
