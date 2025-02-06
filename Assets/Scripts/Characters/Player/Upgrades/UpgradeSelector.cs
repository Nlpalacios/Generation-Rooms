using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeSelector : MonoBehaviour
{
    [Header("Upgrades Icons")]
    [SerializeField] private UpgradeIcons icons;

    [Header("Improvement probability")]
    [SerializeField] private float newAbilityChance = 0.2f;
    [SerializeField] private float statUpgradeChance = 0.5f;
    [SerializeField] private float abilityUpgradeChance = 0.3f;

    private List<PlayerBasicStats> currentUpgrades = new List<PlayerBasicStats>();

    private const float HEART_RESTORE_CHANCE = 0.7f;

    public UpgradeData GenerateData()
    {
        UpgradeData data = new UpgradeData();
        //float roll = Random.value;

        var posibleUpgrades = GetPosiblePlayerUpgrade();
        if (posibleUpgrades.Count == 0) return null;

        //PROBAR - PARA QUE NO SIEMPRE SALGA MEJORA DE CORAZON
        float totalChance = posibleUpgrades.Values.Sum();
        float randomRoll = Random.value * totalChance;

        float cumulativeChance = 0f;
        PlayerBasicStats upgrade = PlayerBasicStats.speed;

        foreach (var entry in posibleUpgrades)
        {
            cumulativeChance += entry.Value;
            if (randomRoll <= cumulativeChance)
            {
                upgrade = entry.Key;
                break;
            }
        }

        data = ConfigurePlayerUpgradeData(upgrade);
        currentUpgrades.Add(upgrade);
        return data;

        //AÑADIRLA DESPUES DE IMPLEMENTAR AL MENOS 2 HABILIDADES
        ////Player Upgrade
        //if (roll < statUpgradeChance)
        //{
           
        //}

        ////Ability Upgrade
        //else if (roll > statUpgradeChance && roll < (statUpgradeChance + abilityUpgradeChance) 
        //    && AbilityManager.Instance.HasAbilities())
        //{

        //    return data;
        //}

        ////New Ability
        //return data;

    }

    private UpgradeData ConfigurePlayerUpgradeData(PlayerBasicStats upgrade)
    {
        UpgradeData data = new UpgradeData
        {
            upgradeType = UpgradeType.playerUpgrade,
            playerUpgrades = upgrade,
            icon = GetPlayerUpgradeIcon(upgrade)
        };

        bool basicUpgrade = true;

        switch (upgrade)
        {
            case PlayerBasicStats.speed:
                data.stat = 0.2f;
                basicUpgrade = false;
                break;

            case PlayerBasicStats.hearts:
                int currentHearts = PlayerStats.Instance.CurrentHearts;
                if (currentHearts < PlayerStats.Instance.GetMaxHearts)
                {
                    float chanceRestoreHearts = Random.value;
                    if (chanceRestoreHearts < HEART_RESTORE_CHANCE)
                    {
                        data.restoreHearts = true;
                        data.stat = 0;
                        basicUpgrade = false;
                    }
                }
                break;

                //In case of Total Cards or Total upgrades are a "Basic Upgrades" and just plus one
        }

        if (basicUpgrade)
        {
            data.stat = 1f;
        }

        return data;
    }

    public Dictionary<PlayerBasicStats, float> GetPosiblePlayerUpgrade()
    {
        var upgrades = new Dictionary<PlayerBasicStats, float>();

        foreach (PlayerBasicStats currentType in Enum.GetValues(typeof(PlayerBasicStats)))
        {
            if (currentUpgrades.Contains(currentType)) continue;
            float difference = PlayerStats.Instance.GetDifferenceStats(currentType);
            if (difference <= 0f) continue;

            upgrades.Add(currentType, difference);
        }

        return upgrades;
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

    public void CompleteGenerate()
    {
        currentUpgrades.Clear();
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