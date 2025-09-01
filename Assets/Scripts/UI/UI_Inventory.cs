using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// プレイヤーのインベントリUIを管理するクラス
public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory; // プレイヤーのインベントリデータ
    private UI_ItemSlot[] uiItemSlots; // アイテムスロットUIの配列
    private UI_EquipSlot[] uiWeaponTrinketSlots; // 武器・アクセサリスロットUI
    private UI_EquipSlot[] uiArmorSlots; // 防具スロットUI

    [SerializeField] private Transform uiItemSlotParent; // アイテムスロットの親オブジェクト
    [SerializeField] private Transform uiWeaponTrinketParent; // 武器・アクセサリスロットの親
    [SerializeField] private Transform uiArmorSlotParent; // 防具スロットの親

    // 初期化
    private void Awake()
    {
        // 各スロットUIを親オブジェクトから取得
        uiItemSlots = uiItemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        uiWeaponTrinketSlots = uiWeaponTrinketParent.GetComponentsInChildren<UI_EquipSlot>();
        uiArmorSlots = uiArmorSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        // プレイヤーインベントリを検索して取得
        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI; // インベントリ更新時にUI更新

        UpdateUI(); // 最初にUIを更新
    }

    // インベントリ全体のUI更新
    private void UpdateUI()
    {
        UpdateInventorySlots();   // アイテムスロット更新
        UpdateEquipmentSlots();   // 装備スロット更新
    }

    // 装備スロットUIの更新
    private void UpdateEquipmentSlots()
    {
        // 武器・アクセサリスロット
        for (int i = 0; i < uiWeaponTrinketSlots.Length; i++)
        {
            if (i < inventory.equipList.Count)
            {
                var slot = inventory.equipList[i];
                uiWeaponTrinketSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }

        // 防具スロット
        for (int i = 0; i < uiArmorSlots.Length; i++)
        {
            int equipListIndex = i + 4; // 防具スロットはequipListの4番目以降
            if (equipListIndex < inventory.equipList.Count)
            {
                var slot = inventory.equipList[equipListIndex];
                uiArmorSlots[i].UpdateSlot(slot.HasItem() ? slot.equippedItem : null);
            }
        }
    }

    // アイテムスロットUIの更新
    private void UpdateInventorySlots()
    {
        List<Inventory_Item> itemList = inventory.itemList;

        for (int i = 0; i < uiItemSlots.Length; i++)
        {
            if (i < itemList.Count)
            {
                uiItemSlots[i].UpdateSlot(itemList[i]); // アイテムがあれば表示
            }
            else
            {
                uiItemSlots[i].UpdateSlot(null); // 空スロットにする
            }
        }
    }

    // インベントリUIを強制的に更新（レイアウトも即時再構築）
    public void RefreshInventoryUI()
    {
        UpdateInventorySlots();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
