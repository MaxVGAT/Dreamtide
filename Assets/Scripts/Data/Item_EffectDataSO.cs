using UnityEngine;


public class Item_EffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    protected Entity_Player player;

    public virtual bool CanBeUsed()
    {
        return true;
    }

    public virtual void ExecuteEffect()
    {

    }

    public virtual void Subscribe(Entity_Player player)
    {
        this.player = player;
    }

    public virtual void Unsubscribe()
    {
    }

}
