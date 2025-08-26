using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private UI_SkillTooltip skillTooltip;

    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    public Player_SkillManager skillManager { get; private set; }

    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    [ContextMenu("Refund all skills")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
            node.Refund();
    }

    public UI_SkillTooltip SkillTooltip => skillTooltip;

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    public void RemoveSkillPoint(int cost) => skillPoints -= cost;
    public void AddSkillPoints(int points) => skillPoints = skillPoints + points;

    private void Start()
    {
        UpdateAllConnections();
    }

    public void UpdateAllConnections()
    {
        foreach(var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }
}
