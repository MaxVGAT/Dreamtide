using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
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
        ui.craftUI.SetupCraftUI(storage);
        ui.SetInsideCraftTrigger(true); // Set trigger state
        ui.OpenInventoryWithCraft();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);

        ui.SetInsideCraftTrigger(true); // Set trigger state when entering
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideCraftTrigger(false); // Clear trigger state and hide storage
        if (ui.IsMenuOpen())
        {
            ui.ToggleUI(); // Close menu when exiting trigger
        }
    }
}
