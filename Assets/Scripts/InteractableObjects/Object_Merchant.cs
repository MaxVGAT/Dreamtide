using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchantInventory;

    private NPC_SFX npcSFX;
    private AudioSource audioSource;
    protected override void Awake()
    {
        base.Awake();
        merchantInventory = PersistentStorageManager.instance.GetMerchantInventory();
        audioSource = GetComponent<AudioSource>();
        npcSFX = GetComponent<NPC_SFX>();

    }


    public void Interact()
    {
        inventory = player.GetComponent<Inventory_Player>();
        merchantInventory.SetInventory(inventory);

        ui.merchantUI.SetupMerchantUI(merchantInventory, inventory);

        ui.SetInsideMerchantTrigger(true);
        npcSFX?.PlayTalkSfx();

        if (!ui.IsMenuOpen())
            ui.OpenInventoryWithMerchant();
        else
            ui.ShowMerchantInInventory(true);
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        ui.SetInsideMerchantTrigger(true); // Set trigger state when entering

        if (ui.IsMenuOpen())
            ui.ShowMerchantInInventory(false);

        audioSource.Play();
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideMerchantTrigger(false); // Clear trigger state and hide storage

        if (ui.IsMenuOpen())
        {
            ui.ToggleUI(); // Close menu when exiting trigger
        }

        audioSource.Stop();
    }
}
