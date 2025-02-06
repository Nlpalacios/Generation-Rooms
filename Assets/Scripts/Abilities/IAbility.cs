using UnityEngine;

public interface IAbility
{
    public TypeAbility GetTypeAbility();
    public void Initialize(SO_Ability data);
    public void ActivateAbility();
    public void DeactivateAbility();
    public bool IsAbilityActive();
    public void UpgradeAbility();
    public void SetData(AbilityUpgrades typeData, int newData, bool sum = true);
}
