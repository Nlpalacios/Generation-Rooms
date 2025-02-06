using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewUpgradeAbility", menuName = "Scriptable Objects/SO_Upgrade")]
public class SO_AbilityStat : ScriptableObject, IAbilityData 
{
    public PlayerBasicStats upgradeStats;
    public int value;

    public void SetStat<T>(T stat, int value)
    {
        this.value = value;
    }
    public int GetStat<T>(T stat)
    {
        return this.value;
    }
    
    public PlayerBasicStats GetData()
    {
        return upgradeStats;
    }
    public TypeAbility GetUpgrade()
    {
        return TypeAbility.None;
    }
}
