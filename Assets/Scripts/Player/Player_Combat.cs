using UnityEngine;

// プレイヤー用の戦闘クラス
public class Player_Combat : Entity_Combat
{
    public Transform counteredTargetTransform { get; private set; } // カウンター対象のTransform

    [Header("Counter Attack details")]
    [SerializeField] private float counterRecovery = 1f; // カウンター後のリカバリー時間

    // カウンター攻撃を実行
    public bool CounterAttackPerformed(out bool isCrit)
    {
        bool hasPerformedCounter = false;
        counteredTargetTransform = null;
        isCrit = false;

        // 周囲の対象を取得してカウンター可能か確認
        foreach (var target in GetDetectedColliders())
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if (counterable.CanBeCountered)
            {
                counteredTargetTransform = target.transform;

                float damage = Stats.GetPhysicalDamage(out isCrit); // 物理ダメージ計算
                counterable.HandleCounterAttack(); // カウンター処理実行
                hasPerformedCounter = true;
                break;
            }
        }
        return hasPerformedCounter;
    }

    // カウンター後の回復時間を取得
    public float GetCounterRecovery() => counterRecovery;
}
