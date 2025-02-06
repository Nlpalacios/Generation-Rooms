using UnityEngine;

[CreateAssetMenu(fileName = "SO_NewAbility", menuName = "Scriptable Objects/SO_Ability")]
public class SO_Ability : ScriptableObject, IAbilityData
{
    [Header("Type")]
    public TypeAbility type;

    [Header("Icon")]
    public Sprite icon;

    [Header("Values")]
    public int exp = 0;
    public int damage = 0;
    public int delay = 0;
    public int range = 0;
    public int seconds = 0;
    public int timeLoad = 0;

    public int rangeUpgrade = 1;

    [Header("Visual Effects")]
    public GameObject effect;

    public void SetStat<T>(T stat, int value)
    {
        switch (stat)
        {
            case AbilityUpgrades.damage: damage = value; break;
            case AbilityUpgrades.range: range = value; break;
            case AbilityUpgrades.timeLoad: timeLoad = value; break;
            case AbilityUpgrades.exp: exp = value; break;
            case AbilityUpgrades.seconds: seconds = value; break;
        }
    }
    public int GetStat<T>(T stat)
    {
        switch (stat)
        {
            case AbilityUpgrades.damage: return damage;
            case AbilityUpgrades.range: return range;
            case AbilityUpgrades.timeLoad: return timeLoad;
            case AbilityUpgrades.exp: return exp;
            case AbilityUpgrades.seconds: return seconds;
            default: return 0;
        }
    }

    public PlayerBasicStats GetData()
    {
        return PlayerBasicStats.None;
    }
    public TypeAbility GetUpgrade()
    {
        return type;
    }
}