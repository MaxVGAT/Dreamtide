using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    private UI ui;
    private RectTransform rect;

    [SerializeField] private Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    private string lockedColorHex = "#5A5A5A";

    private Color lastColor;
    public bool isUnlocked;
    public bool isLocked;


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void UnlockSkill()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;

        return true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastColor = skillIcon.color;

        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            UnlockSkill();
        else
            Debug.Log("Can not be unlocked");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, skillData);

        if(isUnlocked == false)
            UpdateIconColor(Color.white * 0.8f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if(isUnlocked == false)
            UpdateIconColor(lastColor);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);

        return color;
    }
    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.skillName;
        skillIcon.sprite = skillData.skillIcon;
        gameObject.name = "UI_TreeNode - " + skillData.skillName;
    }
}
