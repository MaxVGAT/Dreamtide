using UnityEngine;

public class Item_EffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect()
    {

    }

}
