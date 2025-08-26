using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject tabMenuRoot;
    [SerializeField] private UI_SkillTree skillTree;

    private bool menuEnabled;

    private void Awake()
    {
        tabMenuRoot.SetActive(false);

        if(skillTree == null)
            skillTree = FindAnyObjectByType<UI_SkillTree>();
    }

    public void ToggleUI()
    {
        menuEnabled = !menuEnabled;
        tabMenuRoot.SetActive(menuEnabled);
    }
}
