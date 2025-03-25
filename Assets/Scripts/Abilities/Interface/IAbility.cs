
public interface IAbility
{
    public NameAbility GetTypeAbility();
    public void Initialize(AbilityBaseData data);
    public void ActivateAbility();
    public void DeactivateAbility();
    public bool IsAbilityActive();
    public void UpgradeAbility();
    public void SetData(AbilityUpgrades typeData, int newData, bool sum = true);
}
