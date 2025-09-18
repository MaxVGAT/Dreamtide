using UnityEngine;

public class Object_Banker : Object_NPC, IInteractable
{
    private Inventory_Player inventory;   // プレイヤーのインベントリ参照
    private Inventory_Storage storage;    // 銀行用ストレージ

    private NPC_SFX npcSFX;               // NPC用SFX
    private AudioSource audioSource;      // 音声再生用

    protected override void Awake()
    {
        base.Awake();
        storage = PersistentStorageManager.instance.GetStorageInventory(); // ストレージ取得
        audioSource = GetComponent<AudioSource>();                          // AudioSource取得
        npcSFX = GetComponent<NPC_SFX>();                                   // NPC SFX取得
    }

    // プレイヤーとのインタラクト処理
    public void Interact()
    {
        inventory = player.GetComponent<Inventory_Player>(); // プレイヤーインベントリ取得
        storage.SetInventory(inventory);                      // ストレージにプレイヤー情報セット

        ui.SetInsideShopTrigger(true);                        // 銀行トリガー有効化
        npcSFX?.PlayTalkSfx();                               // 会話音再生

        // 距離減衰無効化
        var distanceController = GetComponent<AudioDistanceController>();
        if (distanceController != null)
            distanceController.ignoreDistance = true;

        if (!ui.IsMenuOpen())
            ui.OpenInventoryWithStorage();                   // UI開く
        else
            ui.ShowStorageInInventory(storage);             // 表示のみ更新

        if (storage != null && ui.storageUI != null)
            ui.storageUI.SetupStorage(storage);             // UIセットアップ
    }

    // トリガー離脱時の処理
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideShopTrigger(false);                      // トリガー解除
        if (ui.IsMenuOpen())
            ui.ToggleUI();                                   // UI閉じる

        // 距離減衰再有効化
        var distanceController = GetComponent<AudioDistanceController>();
        if (distanceController != null)
            distanceController.ignoreDistance = false;

        audioSource.Stop();                                  // 音声停止
    }
}
