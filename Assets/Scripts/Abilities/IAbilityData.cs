public interface IAbilityData 
{
    public int GetStat<T>(T stat);
    public void SetStat<T>(T stat, int value);
    public PlayerBasicStats GetData();
    public TypeAbility GetUpgrade();
}
