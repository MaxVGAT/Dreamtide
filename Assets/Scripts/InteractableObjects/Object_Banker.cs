using UnityEngine;

public class Object_Banker : Object_NPC, IInteractable
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
        ui.SetInsideShopTrigger(true); // Set trigger state
        ui.OpenInventoryWithStorage();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);

        ui.SetInsideShopTrigger(true); // Set trigger state when entering
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideShopTrigger(false); // Clear trigger state and hide storage
        if (ui.IsMenuOpen())
        {
            ui.ToggleUI(); // Close menu when exiting trigger
        }
    }
}
