using UnityEngine;

public class AbilityBasicData
{ 
    public UpgradeType upgradeType;

    public string name;
    public string description;

    public NameAbility typeAbility;
    public AbilityValues abilityValues;

    public PlayerBasicStats playerUpgrades;
    public AbilityUpgrades abilityUpgrades;

    public Sprite icon;
    public float valueUpgrade = 0;

    //Hearts
    public bool restoreHearts = false;
    public bool singleUse = false;
}

public enum UpgradeType
{
    None,
    playerUpgrade,
    AbilityUpgrade,
    NewAbility
}

public enum AbilityType 
{
    PlayerUpgrade,
    NewAbility,
    AbilityUpgrade,
}



public enum PlayerBasicStats
{
    None,
    speed,
    hearts,
    totalCards,
    totalUpgrades,
}

public enum NameAbility
{
    None,
    Ability_Ray,
    Ability_Rock
}

public enum AbilityUpgrades
{
    None,
    damage,
    delay,
    range,
    exp
}
