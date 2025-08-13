using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]

public class Stats
{
    [SerializeField] public float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool wasModified = true;
    private float finalValue;

    public float GetValue()
    {
        if(wasModified)
        {
            finalValue = GetFinalValue();
            wasModified = false;
        }

        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        wasModified = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(mod => mod.source == source);
        wasModified = true;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach(var mod in modifiers)
        {
            finalValue += mod.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]

public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
