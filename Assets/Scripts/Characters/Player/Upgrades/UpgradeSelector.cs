using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class UpgradeSelector : MonoBehaviour
{
    [Header("Upgrades Icons")]
    [SerializeField] private UpgradeIcons icons;

    [Header("Improvement probability")]
    [SerializeField] private Percentages constantPercents;
    private Percentages currentPercents;

    [Serializable]
    public class Percentages
    {
        public float newAbilityPercent = 0.2f;
        public float playerUpgradePercent = 0.5f;
        public float abilityUpgradePercent = 0.3f;
    }
    private HashSet<NameAbility> invalidAbilities = new HashSet<NameAbility>();
    private readonly float HEART_RESTORE_CHANCE = 0.1f;

    //Preferences
    [Header("Preferences")]
    [SerializeField] private int totalAbilityCards = 1;
    [SerializeField] private int totalUpgradeCards = 2;

    [SerializeField] private int currentAbilityCards = 0;
    [SerializeField] private int currentUpgradeCards = 0;

    int GetRandomIndex(int max) => Random.Range(0, max);

    private void Start()
    {
        currentPercents = constantPercents;
    }

    public void ResetPreferences()
    {
        currentAbilityCards = 0;
        currentUpgradeCards = 0;

        currentPercents = constantPercents;
    }

    public AbilityBasicData GenerateData()
    {
        const int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            float roll = Random.value;
            attempts++;

            if (roll < currentPercents.newAbilityPercent)
            {
                AbilityBasicData newAbilityData = TryGenerateNewAbility();
                if (newAbilityData != null) return newAbilityData;
            }

            if (roll >= currentPercents.newAbilityPercent && roll < (currentPercents.newAbilityPercent + currentPercents.abilityUpgradePercent)) 
            {
                AbilityBasicData abilityUpgradeData = TryGenerateAbilityUpgrade();
                if (abilityUpgradeData != null) return abilityUpgradeData;
            }

            AbilityBasicData playerUpgradeData = TryGeneratePlayerUpgrade();
            if (playerUpgradeData != null) return playerUpgradeData;
        }

        Debug.Log("GenerateData: No se pudo generar un upgrade válido después de varios intentos.");
        return null;
    }

    private AbilityBasicData TryGeneratePlayerUpgrade()
    {
        int specialUpgrade = GetRandomIndex(100);

        if (specialUpgrade < 10)
        {
            return ConfigurePlayerUpgradeData(PlayerBasicStats.None, 0, true);
        }

        //Player Upgrade
        var posibleUpgrades = PlayerStats.Instance.GetRandomPlayerUpgrade();
        if (posibleUpgrades.Item1 == PlayerBasicStats.None)
        {
            Debug.Log("ERROR - NO PLAYER UPGRADES");
            return null;
        }

        return ConfigurePlayerUpgradeData(posibleUpgrades.Item1, posibleUpgrades.Item2);
    }

    private AbilityBasicData TryGenerateAbilityUpgrade()
    {
        if (currentUpgradeCards >= totalUpgradeCards || !AbilityController.Instance.HasAbilities())
        {
            currentPercents.abilityUpgradePercent = 0;
            AdjustProbabilities();

            return null;
        }

        var abilities = AbilityController.Instance.GetCurrentAbilities()
                   .Where(ability => !invalidAbilities.Contains(ability))
                   .ToList();

        if (abilities.Count <= 0) return null;

        int randomAbilityIndex = GetRandomIndex(abilities.Count);
        NameAbility ability = abilities[randomAbilityIndex];
        AbilityUpgrades upgrade = SelectUpgrade(ability);

        if (upgrade == AbilityUpgrades.None)
        {
            invalidAbilities.Add(ability);
            return null;
        }

        currentUpgradeCards++;
        return ConfigureAbilityUpgradeData(ability, upgrade);
    }

    private AbilityBasicData TryGenerateNewAbility()
    {
        List<NameAbility> totalAbilities = Enum.GetValues(typeof(NameAbility)).Cast<NameAbility>()
               .Where(type => type != NameAbility.None).ToList();

        List<NameAbility> availableAbilities = totalAbilities
            .Where(type => !AbilityController.Instance.HasAbility(type)).ToList();

        if (AbilityController.Instance.GetCurrentAbilities().Count == totalAbilities.Count - 1 || 
            availableAbilities.Count == 0 || currentAbilityCards >= totalAbilityCards)
        {
            currentPercents.newAbilityPercent = 0;
            AdjustProbabilities();

            return null;
        }

        int randomIndex = GetRandomIndex(availableAbilities.Count);
        NameAbility newAbility = availableAbilities[randomIndex];

        currentAbilityCards++;
        return ConfigureNewAbilityData(newAbility);
    }
    private AbilityUpgrades SelectUpgrade(NameAbility ability)
    {
        List<AbilityUpgrades> abilityUpgrades = new List<AbilityUpgrades>();

        AbilityBaseData currentAbility = AbilityController.Instance.database.GetAbilityData(ability);
        SO_NewAbility dataAbility = null;

        if (currentAbility != null && currentAbility is SO_NewAbility)
        {
            dataAbility = (SO_NewAbility)currentAbility;
        }
        else
        {
            Debug.LogError("NOT FIND UPGRADE DATA");
            return AbilityUpgrades.None;
        }

        foreach (AbilityUpgrades type in Enum.GetValues(typeof(AbilityUpgrades)))
        {
            if (type == AbilityUpgrades.None) continue;

            if (dataAbility.ValidUpgrade(type))
            {
                abilityUpgrades.Add(type);
            }
        }

        if (abilityUpgrades.Count <= 0) return AbilityUpgrades.None;
        return abilityUpgrades[GetRandomIndex(abilityUpgrades.Count)];
    }

    private AbilityBasicData ConfigureNewAbilityData(NameAbility ability)
    {
        AbilityBaseData currentAbility = AbilityController.Instance.database.GetAbilityData(ability);
        if (currentAbility == null) { Debug.LogError("NOT FIND UPGRADE DATA"); return null; }
         
        AbilityBasicData data = new AbilityBasicData
        {
            name = currentAbility.abilityName,
            description = currentAbility.description,

            upgradeType = UpgradeType.NewAbility,
            typeAbility = ability,

            playerUpgrades = PlayerBasicStats.None,
            abilityUpgrades = AbilityUpgrades.None,

            icon = currentAbility.icon,
        };

        return data;
    }
    private AbilityBasicData ConfigureAbilityUpgradeData(NameAbility ability, AbilityUpgrades upgradeData)
    {
        SO_NewAbility dataAbility = null;
        AbilityBaseData currentAbility = AbilityController.Instance.database.GetAbilityData(ability);

        if (currentAbility  != null && currentAbility is SO_NewAbility)
        {
            dataAbility = (SO_NewAbility)currentAbility;
        } 
        else
        {
            Debug.LogError("NOT FIND UPGRADE DATA");
            return null; 
        }

        AbilityBasicData data = new AbilityBasicData
        {
            upgradeType = UpgradeType.AbilityUpgrade,
            playerUpgrades = PlayerBasicStats.None,

            typeAbility = ability,
            abilityUpgrades = upgradeData,

            valueUpgrade = GetRandomIndex(dataAbility.GetMaxValues(upgradeData)),
            icon = dataAbility.icon,
        };

        return data;
    }

    private AbilityBasicData ConfigurePlayerUpgradeData(PlayerBasicStats upgrade, float value, bool specialUpgrade = false)
    {
        AbilityBasicData data = null;

        if (specialUpgrade)
        {
            SO_PlayerAbility specialPlayerUpgrade = AbilityController.Instance.database.GetRandomPlayerAbility();

            data = new AbilityBasicData
            {
                upgradeType = UpgradeType.playerUpgrade,

                playerUpgrades = specialPlayerUpgrade.type,
                valueUpgrade = specialPlayerUpgrade.upgradeValue,

                name = specialPlayerUpgrade.name,
                description = specialPlayerUpgrade.description,

                singleUse = specialPlayerUpgrade.singleUse,
                restoreHearts = specialPlayerUpgrade.restoreHearts,

                icon = specialPlayerUpgrade.icon
            };

            return data;
        }

        AbilityValues valuesUpgrade = new AbilityValues()
        {
            expCost = GetRandomIndex(50),
            delay   = GetRandomIndex(150),

            range   = 0,
            damage = 0,
        };

        if (valuesUpgrade == null) { Debug.LogError("NOT FIND PLAYER UPGRADES VALUES"); return null; }

        data = new AbilityBasicData
        {
            upgradeType = UpgradeType.playerUpgrade,

            playerUpgrades = upgrade,
            abilityValues = valuesUpgrade,
            valueUpgrade = value,

            singleUse = specialUpgrade,
            icon = GetPlayerUpgradeIcon(upgrade)
        };

        if (upgrade == PlayerBasicStats.totalCards || upgrade == PlayerBasicStats.totalUpgrades)
        {
            data.singleUse = true;
        }

        switch (upgrade)
        {
            case PlayerBasicStats.hearts:

                float chanceRestoreHearts = Random.value;
                if (chanceRestoreHearts < HEART_RESTORE_CHANCE)
                {
                    data.restoreHearts = true;
                    data.valueUpgrade = 0;
                }

            break;
        }

        float chanceSingleUse = Random.value;
        if (chanceSingleUse < 0.6)
        {
            data.singleUse = true;
        }

        return data;
    }

    private Sprite GetPlayerUpgradeIcon(PlayerBasicStats typeUpgrade)
    {
        Sprite icon = null;

        switch (typeUpgrade)
        {
            case PlayerBasicStats.speed:
                icon = icons.iconSpeed; 
                break;
            case PlayerBasicStats.hearts:
                icon = icons.iconHearts;
                break;
            case PlayerBasicStats.totalUpgrades:
                icon = icons.iconTotalUpgrades;
                break;
            case PlayerBasicStats.totalCards:
                icon = icons.iconTotalCards;
                break;
        }

        return icon;
    }

    private void AdjustProbabilities()
    {
        float total = currentPercents.newAbilityPercent + currentPercents.playerUpgradePercent + currentPercents.abilityUpgradePercent;
        if (total <= 0f) return;

        float scale = 1f / total;
        currentPercents.newAbilityPercent *= scale;
        currentPercents.playerUpgradePercent *= scale;
        currentPercents.abilityUpgradePercent *= scale;

        //Debug.Log($"Adjusted Probabilities -> New Ability: {currentPercents.newAbilityPercent}, Player Upgrade: {currentPercents.playerUpgradePercent}, Ability Upgrade: {currentPercents.abilityUpgradePercent}");
    }
}

[Serializable]
public class UpgradeIcons
{
    public Sprite iconSpeed;
    public Sprite iconHearts;
    public Sprite iconTotalUpgrades;
    public Sprite iconTotalCards;
}