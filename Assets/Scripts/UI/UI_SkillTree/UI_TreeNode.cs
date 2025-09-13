using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // References
    private RectTransform rect;               // This node's RectTransform
    private UI_SkillTree skillTree;          // Parent skill tree reference
    private UI_TreeConnectHandler connectHandler; // Handles visual connection lines
    private UI_SkillTooltip skillToolTip;    // Tooltip reference

    [Header("Unlock Details")]
    public UI_TreeNode[] neededNodes;        // Nodes that must be unlocked first
    public UI_TreeNode[] conflictNodes;      // Nodes that are blocked when this node is unlocked
    public bool isUnlocked;                  // True if this node has been unlocked
    public bool isLocked;                    // True if this node is locked due to conflicts

    [Header("Skill Details")]
    public Skill_DataSO skillData;           // Skill data SO
    [SerializeField] private Image skillIcon; // Node icon
    [SerializeField] private int skillCost;  // Skill point cost
    private string lockedColorHex = "#5A5A5A"; // Icon color when locked
    private Color lastColor;                 // Stores last applied color

    private void Awake()
    {
        // Cache references
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandler = GetComponent<UI_TreeConnectHandler>();
        skillToolTip = skillTree.SkillTooltip;

        // Set initial color to locked
        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void Start()
    {
        // If skill is unlocked by default, unlock it at start
        if (skillData.unlockedByDefault)
            UnlockSkill();
    }

    private void OnValidate()
    {
        // Update editor display when skillData is assigned
        if (skillData == null) return;

        skillIcon.sprite = skillData.skillIcon;
        skillCost = skillData.cost;
        gameObject.name = "UI_TreeNode - " + skillData.skillName;
    }

    private void OnDisable()
    {
        // Reset icon color when node is disabled
        if (isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        else if (isUnlocked)
            UpdateIconColor(Color.white);
        else
            UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    // Called when node is clicked
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            UnlockSkill();              // Unlock if possible
        else if (isUnlocked)
            skillToolTip.UnlockedSkillEffect(); // Blink unlocked tooltip
        else if (isLocked)
            skillToolTip.LockedSkillEffect();   // Blink locked tooltip
    }

    // Show tooltip when hovering over node
    public void OnPointerEnter(PointerEventData eventData)
    {
        bool hasEnoughPoints = skillTree.EnoughSkillPoints(skillData.cost);
        skillToolTip.ShowToolTip(true, rect, this, skillData, hasEnoughPoints);

        // Highlight node if not unlocked or locked
        if (!isUnlocked && !isLocked)
            ToggleNodeHighlight(true);
    }

    // Hide tooltip when exiting hover
    public void OnPointerExit(PointerEventData eventData)
    {
        skillToolTip.ShowToolTip(false, rect);

        // Remove highlight
        if (!isUnlocked && !isLocked)
            ToggleNodeHighlight(false);
    }

    // Unlock this skill node
    public void UnlockSkill()
    {
        if (isUnlocked)
        {
            Debug.Log("Skill is already unlocked");
            return;
        }

        isUnlocked = true;
        UpdateIconColor(Color.white);                // Make icon white
        skillTree.RemoveSkillPoint(skillData.cost); // Deduct skill points
        LockConflictNodes();                         // Lock conflicting nodes
        connectHandler.UnlockConnectionImage(true); // Unlock visual connection

        // Apply skill upgrade in player skill manager
        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
    }

    public void UnlockWithSaveData()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();

        // Add null check for connectHandler
        if (connectHandler != null)
            connectHandler.UnlockConnectionImage(true);
    }

    // Refund node
    public void Refund()
    {

        if (isUnlocked == false || skillData.unlockedByDefault)
            return;

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);  // Return points
        connectHandler.UnlockConnectionImage(false); // Reset connection visuals

        foreach (var node in conflictNodes)
        {
            UnlockRefundedNode(node);
        }
    }

    private void UnlockRefundedNode(UI_TreeNode node)
    {
        if (node.isLocked)
            node.isLocked = false;

        // Add null check for connectHandler
        if (node.connectHandler != null)
        {
            foreach (var child in node.connectHandler.GetChildNodes())
            {
                UnlockRefundedNode(child);
            }
        }
    }


    // Check if node can be unlocked
    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked) return false;                  // Already unlocked or locked
        if (!skillTree.EnoughSkillPoints(skillData.cost)) return false; // Not enough points

        // All required nodes must be unlocked
        foreach (var node in neededNodes)
            if (!node.isUnlocked) return false;

        // None of the conflicting nodes should be unlocked
        foreach (var node in conflictNodes)
            if (node.isUnlocked) return false;

        return true;
    }

    // Lock all conflicting nodes
    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            node.LockChildNodes(); // Recursively lock child nodes
        }
    }

    // Recursively lock all child nodes
    public void LockChildNodes()
    {
        isLocked = true;

        // Add null check for connectHandler
        if (connectHandler != null)
        {
            foreach (var node in connectHandler.GetChildNodes())
            {
                node.LockChildNodes();
            }
        }
    }

    // Update the icon color
    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null) return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    // Toggle highlight when hovering
    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * 9f;
        highlightColor.a = 1;
        UpdateIconColor(highlight ? highlightColor : lastColor);
    }

    // Convert hex color string to Unity Color
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);
        return color;
    }
}
