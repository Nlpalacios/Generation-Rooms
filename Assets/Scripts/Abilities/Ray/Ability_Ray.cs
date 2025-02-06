using UnityEngine;

public class Ability_Ray : BaseAbilitiy
{
    private RayObject rayAbility;

    public override void Initialize(SO_Ability data)
    {
        base.InitializeFindEnemiesComponent();
        base.Initialize(data);

        currentAbility = TypeAbility.Ability_Ray;
        rayAbility = PrefabAbility.GetComponent<RayObject>();
        if (rayAbility == null) { Debug.LogError("No effect in DATA"); return; }
    }

    public override void ActivateAbility()
    {
        if (rayAbility == null)
        {
            Debug.LogError("Ray effect prefab is not assigned!");
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