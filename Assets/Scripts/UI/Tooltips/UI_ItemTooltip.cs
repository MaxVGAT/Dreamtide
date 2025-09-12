using TMPro;
using UnityEngine;

// アイテムのツールチップUIを管理するクラス
public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;   // アイテム名
    [SerializeField] private TextMeshProUGUI itemRarity; // レアリティ
    [SerializeField] private TextMeshProUGUI itemType;   // アイテム種別
    [SerializeField] private TextMeshProUGUI itemInfo;   // ステータス詳細
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Transform merchantInfo;
    [SerializeField] private Transform inventoryInfo;

    // ツールチップの表示・非表示
    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow, bool buyPrice = false, bool showMerchantInfo = false)
    {
        // Early exit if no item or not showing
        if (!show || itemToShow == null)
        {
            base.ShowToolTip(false, targetRect);
            if (itemName != null) itemName.text = "";
            if (itemType != null) itemType.text = "";
            if (itemInfo != null) itemInfo.text = "";
            if (itemRarity != null) itemRarity.text = "";
            if (itemPrice != null) itemPrice.text = "";
            if (merchantInfo != null) merchantInfo.gameObject.SetActive(false);
            return;
        }

        base.ShowToolTip(true, targetRect);

        merchantInfo.gameObject.SetActive(showMerchantInfo);
        inventoryInfo.gameObject.SetActive(!showMerchantInfo);

        int price = showMerchantInfo ? itemToShow.buyPrice : Mathf.FloorToInt(itemToShow.sellPrice);
        int totalPrice = price * itemToShow.stackSize;
        string fullStackPrice = $"値段: {price}x{itemToShow.stackSize} - {totalPrice}G";
        string singleStackPrice = $"値段: {price}G";

        if (itemName != null) itemName.text = itemToShow?.itemData?.itemName ?? "";
        if (itemType != null) itemType.text = SetItemTypeJP(itemToShow.itemData.itemType);
        if (itemInfo != null) itemInfo.text = itemToShow.GetItemInfo();
        if (itemPrice != null) itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singleStackPrice;
        SetRarityText(itemToShow.itemData.itemRarity);
    }

    // レアリティ表示を更新
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

    // アイテム種別の日本語表記
    private string SetItemTypeJP(Item_Type type)
    {
        switch (type)
        {
            case Item_Type.Helmet: return "ヘルメット";
            case Item_Type.Shoulders: return "ショルダー";
            case Item_Type.Chest: return "チェスト";
            case Item_Type.Pants: return "ズボン";
            case Item_Type.Cape: return "ケープ";
            case Item_Type.Bracers: return "ブレイサー";
            case Item_Type.Gloves: return "グローブ";
            case Item_Type.Boots: return "ブーツ";
            case Item_Type.Weapon: return "ウェポン";
            case Item_Type.Ring: return "リング";
            case Item_Type.Rune: return "ルーン";
            case Item_Type.Material: return "マテリアル";
            case Item_Type.Consumables: return "コンシュマブル";
            default: return null;
        }
    }
}
