using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect", fileName = "Item Effect Data - Refund All Skills")]
public class ItemEffect_RefundSkillPoints : Item_EffectDataSO
{
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        if (ui == null)
        {
            Debug.LogError("UI object not found in the scene!");
            return;
        }

        if (ui.skillTree == null)
        {
            Debug.LogError("UI.skillTree is not assigned!");
            return;
        }

        ui.skillTree.RefundAllSkills();
    }
}
