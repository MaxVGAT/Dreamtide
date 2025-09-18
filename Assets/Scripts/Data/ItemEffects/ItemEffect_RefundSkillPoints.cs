using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup / Item Data/ Item Effect", fileName = "Item Effect Data - Refund All Skills")]
public class ItemEffect_RefundSkillPoints : Item_EffectDataSO
{
    public override void ExecuteEffect(Entity_Player player)
    {
        // シーン内のUIオブジェクトを検索
        UI ui = FindFirstObjectByType<UI>();
        if (ui == null)
        {
            Debug.LogError("シーン内にUIオブジェクトが見つかりません！");
            return;
        }

        // スキルツリーがアサインされているか確認
        if (ui.skillTree == null)
        {
            Debug.LogError("UI.skillTree が設定されていません！");
            return;
        }

        // 全スキルポイントをリセット
        ui.skillTree.RefundAllSkills();
    }
}
