using System.Collections;
using UnityEngine;

// キャラクターに状態異常（火・氷・雷）を適用・管理するクラス
public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None; // 現在適用中の状態異常

    [Header("Shock details")] // 雷関連の設定（後で非表示予定）
    [SerializeField] private GameObject lightningStrikeVFX; // 雷演出
    [SerializeField] private float currentCharge; // 雷蓄積量
    [SerializeField] private float maxCharge = 1; // 雷最大蓄積量
    private Coroutine shockCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }

    // すべての負のステータスを解除
    public void RemoveAllNegativeEffects()
    {
        StopAllCoroutines();
        currentEffect = ElementType.None;
        entityVfx.StopAllVfx();
    }

    // 状態異常を適用
    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if (element == ElementType.Fire && CanBeApplied(ElementType.Fire))
            ApplyBurnEffect(effectData.burnDuration, effectData.burnDamage);

        if (element == ElementType.Ice && CanBeApplied(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMultiplier);

        if (element == ElementType.Lightning && CanBeApplied(ElementType.Lightning))
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
    }

    // 火傷効果適用
    private void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire); // 火耐性
        float finalDamage = fireDamage * (1 - fireResistance); // 耐性を考慮した最終ダメージ

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    // 火傷コルーチン（定期ダメージ）
    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire); // VFX再生

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration); // 総ティック数
        float damagePerTick = totalDamage / tickCount; // 1ティックあたりダメージ
        float tickInterval = 1f / ticksPerSecond; // ティック間隔

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick); // 定期ダメージ
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    // 氷結効果適用
    private void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance); // 耐性で継続時間減少

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
    }

    // 氷結コルーチン（移動速度低下）
    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntityBy(duration, slowMultiplier); // 移動速度低下
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);
        currentEffect = ElementType.None;
    }

    // 雷効果適用
    private void ApplyShockEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance); // 耐性を考慮して蓄積量調整

        currentCharge += finalCharge; // 蓄積

        if (currentCharge >= maxCharge) // 最大蓄積で雷落下
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        if (shockCo != null)
            StopCoroutine(shockCo);

        shockCo = StartCoroutine(ShockEffectCo(duration));
    }

    // 雷エフェクト停止
    private void StopElectrifyEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }

    // 雷攻撃実行
    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVFX, transform.position, Quaternion.identity); // 雷VFX生成
        entityHealth.ReduceHealth(damage); // ダメージ適用
    }

    // 雷コルーチン
    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);

        yield return new WaitForSeconds(duration);
        StopElectrifyEffect(); // 終了時にリセット
    }

    // 状態異常を適用可能か判定
    public bool CanBeApplied(ElementType element)
    {
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true; // 雷は蓄積可能

        return currentEffect == ElementType.None; // 他は無効中のみ
    }
}
