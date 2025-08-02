using System;
using UnityEngine;

[Serializable]

public class Stats
{
    [SerializeField] public float baseValue;

    public float GetValue()
    {
        return baseValue;
    }
}
