using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect/ Grant Skill Point", fileName = "Item Effect data - Grant Skill Point")]
public class ItemEffect_GrantSkillPoint : Item_EffectDataSO
{
    [SerializeField] private int pointsToAdd; // 付与するスキルポイント数

    public override void ExecuteEffect(Entity_Player player)
    {
        // シーン内のUIを検索し、スキルポイントを加算
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTree.AddSkillPoints(pointsToAdd);
    }
}
