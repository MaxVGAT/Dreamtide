using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Entity_Player player;
    private Player_SkillManager skillManager;

    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

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
        player = FindFirstObjectByType<Entity_Player>();
        skillManager = FindFirstObjectByType<Player_SkillManager>();

        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        player.health.OnHealthUpdate += UpdateHealthBar;
        UpdateLevel();
        UpdateInterface();
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
