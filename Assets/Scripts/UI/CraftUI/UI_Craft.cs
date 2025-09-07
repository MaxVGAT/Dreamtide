using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    // プレイヤーのインベントリスロットを表示するUIへの参照

    private Inventory_Player inventory;
    // プレイヤーのインベントリデータ

    private UI_CraftPreview craftPreviewUI;
    // 作成するアイテムのプレビューを表示するUI

    private UI_CraftSlot[] craftSlots;
    // 必要素材を表示するクラフトスロット

    private UI_CraftListButton[] craftListButtons;
    // レシピに対応するクラフトリストボタン

    public void SetupCraftUI(Inventory_Storage storage)
    {
        // プレイヤーのインベントリを参照し、更新イベントに登録
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;

        // クラフトプレビューパネルを初期化
        craftPreviewUI = GetComponentInChildren<UI_CraftPreview>(true);
        craftPreviewUI.SetupCraftPreview(storage);

        // クラフトリストボタンを設定し、スロットを非表示にする
        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
        // 子オブジェクトからクラフトスロットとレシピボタンを取得
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>();

        // レシピが選択されるまでクラフトスロットを非表示にする
        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        // 各ボタンにスロットを割り当て、クリック時に必要素材を表示できるようにする
        foreach (var button in craftListButtons)
            button.SetCraftSlot(craftSlots);
    }

    // インベントリが変更されたときにUIを更新
    private void UpdateUI() => inventoryParent.UpdateSlots(inventory.itemList);
}
