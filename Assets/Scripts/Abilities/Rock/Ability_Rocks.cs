using UnityEngine;

public class Ability_Rocks : BaseAbilitiy
{
    private RockObject rayAbility;

    public override void Initialize(SO_Ability data)
    {
        base.InitializeFindEnemiesComponent();
        base.Initialize(data);

        currentAbility = TypeAbility.Ability_Rock;
        rayAbility = PrefabAbility.GetComponent<RockObject>();
        if (rayAbility == null) { Debug.LogError("No effect in DATA"); return; }
    }
    public override void ActivateAbility()
    {
        if (rayAbility == null)
        {
            Debug.LogError("Rock effect prefab is not assigned!");
            return;
        }
        FindEnemiesComponent.IntantiateAbilityPrefab(PrefabAbility, GetData().range, GetData().damage);
    }

    public override void DeactivateAbility()
    {
        throw new System.NotImplementedException();
    }
    public override bool IsAbilityActive()
    {
        throw new System.NotImplementedException();
    }
    public override void UpgradeAbility()
    {
        SetData(AbilityUpgrades.range, 1);
    }
}
