using UnityEngine;
using System;

[Serializable]
public class DamageScaleData
{
    // ダメージステータスの倍率
    [Header("ダメージ")]
    public float physical = 1f;
    public float elemental = 1f;

    // 冷却効果の倍率
    [Header("チル")]
    public float chillDuration = 3f;
    public float chillSlowMultiplier = 0.2f;

    // 燃焼効果の倍率
    [Header("バーン")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;

    // 感電効果の倍率
    [Header("ショック")]
    public float shockDuration = 3f;
    public float shockDamageScale = 1f;
    public float shockCharge = 0.4f;
}
