using UnityEngine;


public class Item_EffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual bool CanBeUsed()
    {
        return true;
    }

    public virtual void ExecuteEffect()
    {

    }

}
