using System.Xml.Schema;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectHandler;
    private UI_SkillTooltip skillToolTip;

    [Header("Unlock Details")]
    public UI_TreeNode[] neededNodes; // Required nodes to unlock
    public UI_TreeNode[] conflictNodes; // Locked-out nodes on unlock
    public bool isUnlocked;
    public bool isLocked;


    [Header("Skill details")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    private string lockedColorHex = "#5A5A5A";
    private Color lastColor;


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandler = GetComponent<UI_TreeConnectHandler>();

        skillToolTip = skillTree.SkillTooltip;

        UpdateIconColor(GetColorByHex(lockedColorHex));

    }

    private void Start()
    {
        if (skillData.unlockedByDefault)
            UnlockSkill();
        
    }

    public void Refund()
    {
        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(false);
    }

    private void UnlockSkill()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        skillTree.RemoveSkillPoint(skillData.cost);
        LockConflictNodes();
        connectHandler.UnlockConnectionImage(true);

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
            return false;

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
                return false;
        }

        return true;
    }

    private void LockConflictNodes() // Lock all childs of the clicked node, going until there's no child anymore
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            node.LockChildNodes();
        }
    }

    public void LockChildNodes() // Get the childs and lock them, then lock their childs etc.
    {
        isLocked = true;

        foreach(var node in connectHandler.GetChildNodes())
        {
            node.LockChildNodes();
        }
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
        else if (isUnlocked)
            skillToolTip.UnlockedSkillEffect();
        else if (isLocked)
            skillToolTip.LockedSkillEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool hasEnoughPoints = skillTree.EnoughSkillPoints(skillData.cost);
        skillToolTip.ShowToolTip(true, rect, this, hasEnoughPoints);

        if (!isUnlocked && !isLocked)
            ToggleNodeHightlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillToolTip.ShowToolTip(false, rect);

        if (!isUnlocked && !isLocked)
            ToggleNodeHightlight(false);
    }

    private void ToggleNodeHightlight(bool highlight)
    {
        Color highlightColor = Color.white * 9f; highlightColor.a = 1;
        Color colorToApply = highlight ? highlightColor : lastColor;

        UpdateIconColor(colorToApply);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);

        return color;
    }

    private void OnDisable()
    {
        if (isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        else if (isUnlocked)
            UpdateIconColor(Color.white);
        else
            UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.skillName;
        skillIcon.sprite = skillData.skillIcon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.skillName;
    }
}
