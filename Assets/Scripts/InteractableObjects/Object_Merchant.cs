using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Inventory_Storage>();
    }

    public void Interact()
    {
        ui.SetInsideMerchantTrigger(true);  // Enable craft trigger
        ui.OpenInventoryWithMerchant();      // Opens the craft panel automatically
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Get component from player.gameObject since player is a Transform
        inventory = player.gameObject.GetComponent<Inventory_Player>();
        ui.SetInsideMerchantTrigger(true); // Set trigger state when entering
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideMerchantTrigger(false); // Clear trigger state and hide storage
        if (ui.IsMenuOpen())
        {
            ui.ToggleUI(); // Close menu when exiting trigger
        }
    }
}
