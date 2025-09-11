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
    inventory = player.GetComponent<Inventory_Player>();
    storage.SetInventory(inventory);

    ui.SetInsideCraftTrigger(true);  // Enable craft trigger

    if (!ui.IsMenuOpen())
    {
        ui.OpenInventoryWithCraft();      // Opens the craft panel automatically
        ui.craftUI.SetupCraftUI(storage); // Only setup once on first open
    }
    else if (!ui.IsCraftVisible())
    {
        ui.ShowCraftInInventory(true);    // Just show the panel, no setup
    }
    // If already open and visible, do NOTHING
}

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        ui.SetInsideCraftTrigger(true); // Set trigger state when entering

        if (ui.IsMenuOpen())
            ui.ShowCraftInInventory(false);

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.SetInsideCraftTrigger(false); // Clear trigger state and hide panel

        if (ui.IsMenuOpen())
            ui.ShowCraftInInventory(false);

        if (ui.craftUI != null)
            ui.craftUI.ResetCraftPreview(); // Reset preview here, not on F
    }
}
