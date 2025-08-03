using UnityEngine;

public class Player_Combat : Entity_CombatComponent
{
    public Transform counteredTargetTransform { get; private set; }

    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = 1f;


    public bool CounterAttackPerformed(out bool isCrit)
    {
        bool hasPerformedCounter = false;
        counteredTargetTransform = null;
        isCrit = false;

        foreach(var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if(counterable.CanBeCountered)
            {
                counteredTargetTransform = target.transform;

                float damage = Stats.GetPhysicalDamage(out isCrit);
                counterable.HandleCounterAttack();
                hasPerformedCounter = true;
                break;
            }
        }
        return hasPerformedCounter;
    }

    public float GetCounterRecovery() => counterRecovery;
}
