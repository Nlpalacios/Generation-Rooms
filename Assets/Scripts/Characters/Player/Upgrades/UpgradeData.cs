using UnityEngine;

public class UpgradeData
{
    public UpgradeType upgradeType;
    public PlayerBasicStats playerUpgrades;
    public AbilityUpgrades abilityUpgrades;

    public Sprite icon;
    public float stat = 0;

    //Hearts
    public bool restoreHearts = false;
}

public enum UpgradeType
{
    playerUpgrade,
    AbilityUpgrade,
    NewAbility
}

public enum PlayerBasicStats
{
    None,
    speed,
    hearts,
    totalCards,
    totalUpgrades,
}

public enum AbilityUpgrades
{
    damage,
    delay,
    range,

    timeLoad,
    exp,
    seconds
}

public enum TypeAbility
{
    None,
    Ability_Ray,
    Ability_Rock
}