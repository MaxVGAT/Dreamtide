using UnityEngine;

public class Player_Combat : Entity_CombatComponent
{
    public Transform counteredTargetTransform { get; private set; }

    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = 1f;


    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;
        counteredTargetTransform = null;

        foreach(var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if(counterable.CanBeCountered)
            {
                counterable.HandleCounterAttack();
                hasPerformedCounter = true;
                counteredTargetTransform = target.transform;
            }
        }
        return hasPerformedCounter;
    }

    public float GetCounterRecovery() => counterRecovery;
}
