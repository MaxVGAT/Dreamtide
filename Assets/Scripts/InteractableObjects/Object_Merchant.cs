// 商人NPCクラス
using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    private Inventory_Player inventory;            // プレイヤーインベントリ参照
    private Inventory_Merchant merchantInventory;  // 商人インベントリ参照

    private NPC_SFX npcSFX;                         // NPC専用サウンド
    private AudioSource audioSource;                // BGMやSE用

    protected override void Awake()
    {
        base.Awake();
        merchantInventory = PersistentStorageManager.instance.GetMerchantInventory(); // 商人インベントリ取得
        audioSource = GetComponent<AudioSource>();   // AudioSource取得
        npcSFX = GetComponent<NPC_SFX>();           // NPC SFX取得
    }

    // プレイヤーがインタラクトしたとき
    public void Interact()
    {
        inventory = player.GetComponent<Inventory_Player>();  // プレイヤーのインベントリ取得
        merchantInventory.SetInventory(inventory);           // 商人インベントリと同期

        ui.merchantUI.SetupMerchantUI(merchantInventory, inventory); // UIセットアップ
        ui.SetInsideMerchantTrigger(true);                          // プレイヤーが商人範囲内をセット
        npcSFX?.PlayTalkSfx();                                      // 会話SFX再生

        if (!ui.IsMenuOpen())
            ui.OpenInventoryWithMerchant(); // メニュー開く
        else
            ui.ShowMerchantInInventory(true); // メニュー内に商人表示
    }

    // プレイヤーがトリガーに入ったとき
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        ui.SetInsideMerchantTrigger(true); // 商人範囲内フラグセット

        if (ui.IsMenuOpen())
            ui.ShowMerchantInInventory(false); // メニュー開いてる場合は非表示に

        audioSource.Play(); // 効果音再生
    }

    // プレイヤーがトリガーから出たとき
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        ui.SetInsideMerchantTrigger(false); // 商人範囲外フラグ解除

        if (ui.IsMenuOpen())
        {
            ui.ToggleUI(); // メニューを閉じる
        }

        audioSource.Stop(); // 音停止
    }
}
