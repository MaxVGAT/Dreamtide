using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private RectTransform rect;
    private Button button;

    private Skill_DataSO skillData;

    public Skill_Type skillType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private GameObject conflictSlot;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_SkillSlot - " + skillType.ToString();
    }

    public void SetupSkillSlot(Skill_DataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black; color.a = 0.9f;
        cooldownImage.color = color;

        inputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.skillIcon;

        if (conflictSlot != null)
            conflictSlot.SetActive(false);
    }

    public void StartCooldown(float cooldown)
    {
        cooldownImage.fillAmount = 1f;
        StartCoroutine(CooldownCo(cooldown));
    }

    public void ResetCooldown() => cooldownImage.fillAmount = 0;

    private IEnumerator CooldownCo(float duration)
    {
        float timePassed = 0;

        while(timePassed < duration)
        {
            timePassed += Time.deltaTime;
            cooldownImage.fillAmount = 1f - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillData == null)
            return;

        ui.skillTree.SkillTooltip.ShowToolTip(true, rect, null, skillData, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTree.SkillTooltip.ShowToolTip(false, rect);
    }
}
