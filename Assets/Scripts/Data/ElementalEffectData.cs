using UnityEngine;
using System;

[Serializable]
public class ElementalEffectData // HandleStatusEffect に渡すステータス効果の値をまとめたクラス
{
    public float chillDuration; // チル状態の継続時間（秒）
    public float chillSlowMultiplier; // スロー効果の強さ（0〜1）、1 = 100%スロー

    public float burnDuration; // バーン状態の継続時間（秒）
    public float burnDamage; // バーンダメージのスケーリング値

    public float shockDuration; // 雷チャージが消えるまでの時間
    public float shockDamage; // 雷が命中したときのダメージ
    public float shockCharge; // 攻撃1回あたりに得られるチャージ量（0〜1）

    // 以下のステータスは Entity_Stats から取得し、HandleStatusEffect で計算・使用する
    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)
    {
        chillDuration = damageScale.chillDuration;
        chillSlowMultiplier = damageScale.chillSlowMultiplier;

        burnDuration = damageScale.burnDuration;
        burnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.shockDamageScale;
        shockCharge = damageScale.shockCharge;
    }
}
