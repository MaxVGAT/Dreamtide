using UnityEngine;

// 小型ヘルスバー（ミニヘルスバー）を管理するクラス
public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity; // このヘルスバーが追従するエンティティ

    private void Awake()
    {
        // 親階層からEntityを取得
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        if (entity == null)
            entity = GetComponentInParent<Entity>();

        if (entity != null)
            // Entityが反転した際にヘルスバーを元に戻すイベントに登録
            entity.OnFlipped += HandleMiniHealthBarFlip;
    }

    private void OnDisable()
    {
        if (entity != null)
            // イベント登録解除
            entity.OnFlipped -= HandleMiniHealthBarFlip;
    }

    // エンティティが反転したとき、ヘルスバーの回転をリセット
    private void HandleMiniHealthBarFlip() => transform.rotation = Quaternion.identity;
}
