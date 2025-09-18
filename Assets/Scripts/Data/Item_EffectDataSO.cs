using UnityEngine;

public class Item_EffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription; // 効果の説明文
    protected Entity_Player player;  // 効果対象のプレイヤー参照

    public virtual bool CanBeUsed(Entity_Player player)
    {
        // 使用可能かどうかの判定（必要に応じてオーバーライド）
        return true;
    }

    public virtual void ExecuteEffect(Entity_Player player)
    {
        // アイテム使用時の効果処理（オーバーライド推奨）
    }

    public virtual void Subscribe(Entity_Player player)
    {
        // イベント購読などでプレイヤー参照を保持
        this.player = player;
    }

    public virtual void Unsubscribe()
    {
        // イベント解除や参照解放用
    }
}
