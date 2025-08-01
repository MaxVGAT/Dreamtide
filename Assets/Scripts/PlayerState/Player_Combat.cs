using UnityEngine;

public class Player_Combat : Entity_CombatComponent
{

    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = 1f;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach(var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if(counterable.CanBeCountered)
            {
                counterable.HandleCounterAttack();
                hasPerformedCounter = true;
            }
        }
        return hasPerformedCounter;
    }

    public float GetCounterRecovery() => counterRecovery;
}
