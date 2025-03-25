using UnityEngine;

[CreateAssetMenu(fileName = "New Player Ability", menuName = "Abilities/PLAYER Ability")]
public class SO_PlayerAbility : AbilityBaseData
{
    [Space]
    [Header("Type - Ability")]
    public PlayerBasicStats type;
    public float upgradeValue = 0;

    [Header("Values")]
    public AbilityValues basicValues;

    [Space]
    [Header("Special CASES")]
    public bool isSpecialCase = false;
    public bool restoreHearts = false;
    //Add more in future
}
