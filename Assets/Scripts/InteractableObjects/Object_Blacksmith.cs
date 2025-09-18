using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
{
    private Inventory_Player inventory;   // プレイヤーのインベントリ参照
    private Inventory_Storage storage;    // ブラックスミス用ストレージ

    private NPC_SFX npcSFX;               // NPC用SFX
    private AudioSource audioSource;      // 音声再生用

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Inventory_Storage>();  // ストレージ取得
        audioSource = GetComponent<AudioSource>();    // AudioSource取得
        npcSFX = GetComponent<NPC_SFX>();            // NPC SFX取得
    }

    // プレイヤーとのインタラクト処理
    public void Interact()
    {
        inventory = player.GetComponent<Inventory_Player>();  // プレイヤーのインベントリ取得
        storage.SetInventory(inventory);                       // ストレージにプレイヤー情報セット

        ui.SetInsideCraftTrigger(true);  // クラフトトリガー有効化
        npcSFX?.PlayTalkSfx();           // 会話音再生

        if (!ui.IsMenuOpen())
        {
            ui.OpenInventoryWithCraft();      // クラフトUIを開く
            ui.craftUI.SetupCraftUI(storage); // 初回セットアップ
        }
        else if (!ui.IsCraftVisible())
        {
            ui.ShowCraftInInventory(true);    // 表示のみ更新
        }
        // 開いていて表示中なら何もしない
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        ui.SetInsideCraftTrigger(true); // トリガー状態セット

        if (ui.IsMenuOpen())
            ui.ShowCraftInInventory(false); // UI非表示

        audioSource.Play(); // 効果音再生
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideCraftTrigger(false); // トリガー解除

        if (ui.IsMenuOpen())
            ui.ShowCraftInInventory(false); // UI非表示

        if (ui.craftUI != null)
            ui.craftUI.ResetCraftPreview(); // クラフトプレビューリセット

        audioSource.Stop(); // 音声停止
    }
}
