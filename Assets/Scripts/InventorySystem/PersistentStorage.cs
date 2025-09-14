using UnityEngine;

public class PersistentStorageManager : MonoBehaviour, ISaveable
{
    public static PersistentStorageManager instance;

    [SerializeField] private ItemListDataSO itemDatabase;
    [SerializeField] private Inventory_Storage storageInventory;
    [SerializeField] private Inventory_Merchant merchantInventory;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        // Assign the database to all inventory components
        if (storageInventory != null)
            storageInventory.SetItemDatabase(itemDatabase);

        if (merchantInventory != null)
            merchantInventory.SetItemDatabase(itemDatabase);
    }

    // Expose methods for NPCs in other scenes to use
    public Inventory_Storage GetStorageInventory() => storageInventory;
    public Inventory_Merchant GetMerchantInventory() => merchantInventory;

    public void SaveData(ref GameData data)
    {
        storageInventory?.SaveData(ref data);
        merchantInventory?.SaveData(ref data);
    }

    public void LoadData(GameData data)
    {
        storageInventory?.LoadData(data);
        merchantInventory?.LoadData(data);
    }
}