using System;
using UnityEngine;

public abstract class AbilityBaseData : ScriptableObject
{
    [Header("Name & type")]
    public string abilityName;
    public AbilityType abilityType;

    [Header("Description - DEV")]
    public string description;

    [Header("Icon")]
    public Sprite icon;

    [Header("Use")]
    public bool singleUse = false;
}

[Serializable]
public class AbilityValues
{
    public int expCost = 0;
    public int damage = 0;
    public int delay = 0;
    public int range = 0;
}