using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftData;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftPreview craftPreview;

    public void SetCraftSlot(UI_CraftSlot[] slots, UI_CraftPreview preview)
    {
        craftSlots = slots;
        craftPreview = preview;
    }

    public void UpdateCraftSlots()
    {
        if (craftSlots == null || craftPreview == null)
        {
            Debug.LogError($"[{name}] craftSlots or preview not assigned!");
            return;
        }

        if (craftData == null)
        {
            Debug.Log("You need to assign craft list data!");
            return;
        }

        // Hide all slots
        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        // Populate only slots needed
        for (int i = 0; i < craftData.itemList.Length; i++)
        {
            craftSlots[i].gameObject.SetActive(true);
            craftSlots[i].SetupButton(craftData.itemList[i]);

            // Update preview for first slot automatically
            if (i == 0)
                craftSlots[i].UpdateCraftPreview();
        }
    }
}
