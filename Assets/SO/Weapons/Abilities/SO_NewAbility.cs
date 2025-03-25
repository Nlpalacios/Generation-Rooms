using UnityEngine;

[CreateAssetMenu(fileName = "New Active Ability", menuName = "Abilities/NEW Ability")]
public class SO_NewAbility : AbilityBaseData
{
    [Space]
    [Header("Type - Ability")]
    public NameAbility type;

    [Header("Values")]
    public AbilityValues basicValues;
    [Space]
    public AbilityValues maxUpgradeValues;

    [Header("Visual Effects")]
    public GameObject effect;

    public int GetAbilityValue(AbilityUpgrades ability, bool isMaxValue = false)
    {
        var values = isMaxValue ? maxUpgradeValues : basicValues;

        return ability switch
        {
            AbilityUpgrades.damage => values.damage,
            AbilityUpgrades.delay => values.delay,
            AbilityUpgrades.range => values.range,
            AbilityUpgrades.exp => values.expCost,
            _ => -1
        };
    }
    public int GetCurrentValues(AbilityUpgrades ability) => GetAbilityValue(ability);
    public int GetMaxValues(AbilityUpgrades ability) => GetAbilityValue(ability, true);
    public bool ValidUpgrade(AbilityUpgrades upgrade) => GetCurrentValues(upgrade) != GetMaxValues(upgrade);
}
