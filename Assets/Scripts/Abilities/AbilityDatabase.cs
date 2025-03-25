using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityDatabase : MonoBehaviour
{
    [SerializeField] private List<AbilityBaseData> abilities;

    private Dictionary<NameAbility, SO_NewAbility> abilityDictionary = new Dictionary<NameAbility, SO_NewAbility> ();
    private Dictionary<PlayerBasicStats, SO_PlayerAbility> playerBasicStatsDictionary = new Dictionary<PlayerBasicStats, SO_PlayerAbility> ();
    List<SO_PlayerAbility> playerUpgrades = new List<SO_PlayerAbility> ();

    private void Awake()
    {
        foreach (var ability in abilities)
        {
            if (ability is SO_NewAbility dataAbility)
            {
                abilityDictionary.Add(dataAbility.type, dataAbility);
            }
            else if (ability is SO_PlayerAbility dataPlayer)
            {
                playerBasicStatsDictionary.Add(dataPlayer.type, dataPlayer);
                playerUpgrades.Add(dataPlayer);
            }
        }
    }
    public AbilityBaseData GetAbilityBaseData(string name)
    {
        return abilities.FirstOrDefault(a => a.abilityName == name);
    }

    public SO_NewAbility GetAbilityData(NameAbility type)
    {
        if (abilityDictionary.TryGetValue(type, out SO_NewAbility abilityData) && abilityData is SO_NewAbility ability)
        {
            return ability;
        }

        Debug.LogWarning($"NOT FIND ABILITY: {type}");
        return null;
    }

    public SO_PlayerAbility GetPlayerAbility(PlayerBasicStats type)
    {
        if (playerBasicStatsDictionary.TryGetValue(type, out SO_PlayerAbility abilityData) && abilityData is SO_PlayerAbility ability)
        {
            return ability;
        }

        Debug.LogWarning($"NOT FIND ABILITY: {type}");
        return null;
    }

    public SO_PlayerAbility GetRandomPlayerAbility()
    {
        return playerUpgrades[Random.Range(0, playerBasicStatsDictionary.Count - 1)];
    }
}