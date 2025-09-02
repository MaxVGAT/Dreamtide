using UnityEngine;

// �v���C���[�p�̐퓬�N���X
public class Player_Combat : Entity_Combat
{
    public Transform counteredTargetTransform { get; private set; } // �J�E���^�[�Ώۂ�Transform

    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = 1f; // �J�E���^�[��̃��J�o���[����

    // �J�E���^�[�U������s
    public bool CounterAttackPerformed(out bool isCrit)
    {
        bool hasPerformedCounter = false;
        counteredTargetTransform = null;
        isCrit = false;

        // ���͂̑Ώۂ�擾���ăJ�E���^�[�\���m�F
        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if (counterable.CanBeCountered)
            {
                counteredTargetTransform = target.transform;

                float damage = Stats.GetPhysicalDamage(out isCrit); // �����_���[�W�v�Z
                counterable.HandleCounterAttack(); // �J�E���^�[�������s
                hasPerformedCounter = true;
                break;
            }
        }
        return hasPerformedCounter;
    }

    // �J�E���^�[��̉񕜎��Ԃ�擾
    public float GetCounterRecovery() => counterRecovery;
}
