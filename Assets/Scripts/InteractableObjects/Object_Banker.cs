using UnityEngine;

public class Object_Banker : Object_NPC, IInteractable
{

    private Inventory_Player inventory;
    private Inventory_Storage storage;

    private NPC_SFX npcSFX;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        storage = PersistentStorageManager.instance.GetStorageInventory();
        audioSource = GetComponent<AudioSource>();
        npcSFX = GetComponent<NPC_SFX>();

        
    }

    public void Interact()
    {
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);

        ui.SetInsideShopTrigger(true);
        npcSFX?.PlayTalkSfx();

        // Disable distance effect while talking
        var distanceController = GetComponent<AudioDistanceController>();
        if (distanceController != null)
            distanceController.ignoreDistance = true;

        if (!ui.IsMenuOpen())
            ui.OpenInventoryWithStorage();
        else
            ui.ShowStorageInInventory(storage);

        if (storage != null && ui.storageUI != null)
            ui.storageUI.SetupStorage(storage);
    }

    // When conversation ends / exiting trigger:
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideShopTrigger(false);
        if (ui.IsMenuOpen())
            ui.ToggleUI();

        // Re-enable distance effect
        var distanceController = GetComponent<AudioDistanceController>();
        if (distanceController != null)
            distanceController.ignoreDistance = false;

        audioSource.Stop();
    }

}
