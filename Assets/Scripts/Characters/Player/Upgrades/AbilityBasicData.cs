using UnityEngine;

public class AbilityBasicData
{
    //data
    public SO_PlayerAbility playerAbilityData;
    public SO_NewAbility newAbilityData;
    public UpgradeData upgradeData;

    //type data
    public UpgradeType upgradeType;

    //Uses
    public int totalUses = 0;

    public AbilityBasicData(AbilityBaseData baseData, UpgradeType type)
    {
        if (baseData == null) { Debug.LogWarning("NULL DATA"); return; }

        if (baseData is SO_NewAbility)
        {
            newAbilityData = (SO_NewAbility)baseData;
        }
        else if (baseData is SO_PlayerAbility)
        {
            playerAbilityData = (SO_PlayerAbility)baseData;
            totalUses = playerAbilityData.totalUses;
        }
        else if (baseData is UpgradeData)
        {
            upgradeData = (UpgradeData)baseData;
        }

        upgradeType = type;
    }

    public AbilityBaseData GetBasicData()
    {
        if (playerAbilityData == null && newAbilityData == null)
        {
            Debug.LogError("ALL DATA NULL");
            return null;
        }

        return playerAbilityData != null ? playerAbilityData
                                         : newAbilityData;
    }
    public bool canUseAbility() => totalUses > 0;
    public void UseAbility()
    {
        if (canUseAbility())
        {
            totalUses--;
        }
    }
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