using UnityEngine;
using System;

[Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    public float physical = 1f;
    public float elemental = 1f;

    [Header("Chill")]
    public float chillDuration = 3f;
    public float chillSlowMultiplier = 0.2f;

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;

    [Header("Shock")]
    public float shockDuration = 3f;
    public float shockDamageScale = 1f;
    public float shockCharge = 0.4f;
}
