public interface IAbilityData 
{
    public int GetStat(AbilityUpgrades stat);
    public void SetStat(AbilityUpgrades stat, int value);
    public NameAbility GetTypeAbility();
}