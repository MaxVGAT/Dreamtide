using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;

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

    private Color baseColorHex = new Color(0.6f, 0.6f, 0.6f, 1f);
    private Color unlockedColorHex = Color.white;
    public Color lockedColorHex = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color availableColorHex = new Color(1f, 1f, 1f, 1f);


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();

        UpdateIconColor(baseColorHex);
    }

    private void UnlockSkill()
    {
        if (isLocked)
            return;

        isUnlocked = true;
        skillTree.RemoveSkillPoint(skillData.cost);
        LockConflictNodes();
        UpdateIconColor(unlockedColorHex);
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
            return false;

        foreach(var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }
        
        foreach(var node in conflictNodes)
        {
            if (node.isUnlocked)
                return false;
        }

        return true;
    }

    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            node.UpdateIconColor(node.lockedColorHex);
        }

    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

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
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if (!isUnlocked && !isLocked)
            UpdateIconColor(availableColorHex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (isUnlocked)
            UpdateIconColor(unlockedColorHex);
        else if(isLocked)
            UpdateIconColor(lockedColorHex);
        else
            UpdateIconColor(baseColorHex);
    }

    //private Color GetColorByHex(string hexNumber)
    //{
    //    ColorUtility.TryParseHtmlString(hexNumber, out Color color);

    //    return color;
    //}

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
