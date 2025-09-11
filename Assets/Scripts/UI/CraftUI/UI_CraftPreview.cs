using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Inventory_Item itemToCraft;
    private Inventory_Storage storage;
    private UI_CraftPreviewSlot[] craftPreviewSlots;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI itemRarity;

    public void SetupCraftPreview(Inventory_Storage storage)
    {
        this.storage = storage;

        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewSlot>();

        foreach(var slot in craftPreviewSlots)
            slot.gameObject.SetActive(false);

        if (itemToCraft == null)
            buttonText.text = "なし";
    }

    public void UpdateCraftPreview(Item_DataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemToCraft.GetItemInfo();
        SetRarityText(itemData.itemRarity);
        UpdateCraftPreviewSlots();
    }

    private void UpdateCraftPreviewSlots()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "なし";
            foreach (var slot in craftPreviewSlots)
                slot.gameObject.SetActive(false);
            return;
        }

        buttonText.text = "クラフト";

        foreach (var slot in craftPreviewSlots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < itemToCraft.itemData.craftRecipe.Length; i++)
        {
            Inventory_Item requiredItem = itemToCraft.itemData.craftRecipe[i];
            int availableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);
            int requiredAmount = requiredItem.stackSize;

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupPreviewSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }

    public void ConfirmCraft()
    {
        if(storage.hasEnoughMaterials(itemToCraft) && storage.playerInventory.CanAddItem(itemToCraft))
        {
            storage.ConsumeMaterials(itemToCraft);
            storage.playerInventory.AddItem(itemToCraft);
        }

        UpdateCraftPreviewSlots();
    }

    public void ResetCraftPreview()
    {
        itemToCraft = null;
        foreach (var slot in craftPreviewSlots)
            slot.gameObject.SetActive(false);
        buttonText.text = "なし";
    }

    private void SetRarityText(Item_Rarity rarity)
    {
        var (color, text) = GetRarityColorAndText(rarity);
        itemRarity.text = text;
        itemRarity.color = color;
    }

    // レアリティに応じた色とテキストを取得
    private (Color, string) GetRarityColorAndText(Item_Rarity rarity)
    {
        switch (rarity)
        {
            case Item_Rarity.Common: return (Color.gray, "コモン");
            case Item_Rarity.Uncommon: return (Color.white, "アンコモン");
            case Item_Rarity.Rare: return (Color.blue, "レア");
            case Item_Rarity.Epic: return (new Color(0.64f, 0.21f, 0.93f), "エピック");
            case Item_Rarity.Legendary: return (new Color(1f, 0.5f, 0f), "レジェンダリー");
            case Item_Rarity.Unique: return (new Color(0, 1f, 0.73f), "ユニック");
            default: return (Color.white, "不明");
        }
    }
}
