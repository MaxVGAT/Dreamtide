using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Entity_Player player;
    private Player_SkillManager skillManager;
    private Inventory_Player inventory;

    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Quick Item Slots")]
    [SerializeField] private float yOffsetQuickItemParent = 60;
    [SerializeField] private float xOffsetQuickItemParent = 60;
    [SerializeField] private Transform quickItemOptionsParent;
    [SerializeField] private GameObject closeButton;
    private UI_QuickItemSlotSelection[] quickItemOptions;
    private UI_QuickItemSlot[] quickItemSlots;

    [Header("Experience Details")]
    [SerializeField] AnimationCurve experienceCurve;
    int currentLevel;
    int totalExp;
    int previousLevelsExp;
    int nextLevelsExp;

    [Header("EXP Interface")]
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expValue;

    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();
        player = FindFirstObjectByType<Entity_Player>();
        skillManager = FindFirstObjectByType<Player_SkillManager>();

        inventory = player.inventory;
        inventory.OnInventoryChange += UpdateQuickSlotsUI;
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;

        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        player.health.OnHealthUpdate += UpdateHealthBar;
        UpdateLevel();
        UpdateInterface();
    }

    public void PlayQuickSlotFeedback(int slotNumber) => quickItemSlots[slotNumber].SimulateButtonFeedback();

    public void UpdateQuickSlotsUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        for(int i = 0; i < quickItems.Length; i++)
        {
            if (quickItems[i] != null && inventory.itemList.Contains(quickItems[i]))
            {
                quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
            }
            else
            {
                quickItemSlots[i].UpdateQuickSlotUI(null);
                quickItems[i] = null;
            }
        }
    }

    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        quickItemOptionsParent.gameObject.SetActive(true);

        if (quickItemOptions == null)
            quickItemOptions = quickItemOptionsParent.GetComponentsInChildren<UI_QuickItemSlotSelection>(true);

        List<Inventory_Item> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == Item_Type.Consumables);

        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < consumables.Count)
            {
                Debug.Log("Active");
                quickItemOptions[i].gameObject.SetActive(true); // activate child first
                quickItemOptions[i].SetupOption(quickItemSlot, consumables[i]);
            }
            else
            {
                quickItemOptions[i].gameObject.SetActive(false);
            }
        }
        closeButton.SetActive(true);
        quickItemOptionsParent.position = targetRect.position + new Vector3(xOffsetQuickItemParent, yOffsetQuickItemParent);
    }

    public void HideQuickItemOptions()
    {
        closeButton.SetActive(false);
        quickItemOptionsParent.position = new Vector3(0, 9999);
    }

    public UI_SkillSlot GetSkillSlot(Skill_Type skillType)
    {
        foreach(var slot in skillSlots)
        {
            if (slot.skillType == skillType)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }
        }

        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            AddExperience(150);
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();

        float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);
        if (sizeDifference > 0.1f)
            healthRect.sizeDelta = new Vector2(maxHealth * 0.2f, healthRect.sizeDelta.y);

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;

        healthSlider.value = player.health.GetHealthPercent();
    }

    public void AddExperience(int amount)
    {
        totalExp += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    private void CheckForLevelUp()
    {
        while(totalExp >= nextLevelsExp)
        {
            currentLevel++;
            skillManager.AddSkillPoints(1);
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {
        previousLevelsExp = currentLevel <= 1 ? 0 : Mathf.RoundToInt(experienceCurve.Evaluate(currentLevel));

        nextLevelsExp = Mathf.RoundToInt(experienceCurve.Evaluate(currentLevel + 1));
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = totalExp - previousLevelsExp;
        int end = nextLevelsExp - previousLevelsExp;

        levelText.text = currentLevel.ToString();
        expText.text = start + " / " + end;
        expValue.fillAmount = (float)start / (float)end;
    }
}
