using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private int baseValue;

    private List<int> modifiers = new List<int>();

    public int getValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(i => finalValue *= i);
        return finalValue;
    }

    public void setValue(int value)
    {
        baseValue = value;
    }

    public void addValue(int value)
    {
        baseValue += value;
    }

    public void AddModifier (int modifier)
    {
        if (modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier (int modifier)
    {
        modifiers.Remove(modifier);
    }
}
