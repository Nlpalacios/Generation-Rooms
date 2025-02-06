using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] so_ability;
    private List<IAbilityData> dataAbilities = new List<IAbilityData>();

    private List<IAbility> totalAbilities = new List<IAbility>();
    private List<IAbility> currentAbilities = new List<IAbility>();

    public List<IAbility> CurrentAbilities { get => currentAbilities; set => currentAbilities = value; }
    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        InitScriptableObjects();
        InitializeAbilities();
    }

    private void InitScriptableObjects()
    {
        foreach (var ability in so_ability)
        {
            if (ability == null) continue;

            if (ability is IAbilityData data)
            {
                dataAbilities.Add(data);
            }
        }
    }
    private void InitializeAbilities()
    {
        Ability_Ray abilityRay = new Ability_Ray();
        abilityRay.Initialize(GetDataAbility(TypeAbility.Ability_Ray));

        Ability_Rocks abilityRock = new Ability_Rocks();
        abilityRock.Initialize(GetDataAbility(TypeAbility.Ability_Rock));



        //Add more abilities
        totalAbilities = new List<IAbility> { abilityRay,
                                              abilityRock };

        //TEMPORAL
        AddAbility(TypeAbility.Ability_Ray);
        AddAbility(TypeAbility.Ability_Rock);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            StartAbility(TypeAbility.Ability_Rock);
        }
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        totalAbilities.Clear();
    }

    public bool HasAbilities()
    {
        return CurrentAbilities.Count > 0;
    }
    public bool HasAbility(TypeAbility type)
    {
        return CurrentAbilities.Exists(x => x.GetTypeAbility() == type);
    }
    public void AddAbility(TypeAbility type)
    {
        IAbility ability = GetAbility(type);
        if (ability == null) { Debug.LogError($"NOT FIND ABILITY: {type}"); return; }
        currentAbilities.Add(ability);
    }

    public void StartAbility(TypeAbility type)
    {
        IAbility ability = GetAbility(type);
        if (ability == null) { Debug.LogError($"NOT FIND ABILITY: {type}"); return; }

        ability.ActivateAbility();
    }
    public void StopAbility(TypeAbility type)
    {
        IAbility ability = GetAbility(type);
        if (ability == null) { Debug.LogError($"NOT FIND ABILITY: {type}"); return; }

        ability.DeactivateAbility();
    }

    public IAbility GetAbility(TypeAbility type)
    {
        return totalAbilities.Find(x => x.GetTypeAbility() == type);
    }
    public SO_Ability GetDataAbility(TypeAbility type)
    {
        IAbilityData abilityData = dataAbilities.Find(data => data.GetUpgrade() == type);
        if (abilityData != null && abilityData is SO_Ability ability)
        {
            return ability; 
        }

        return null;
    }
}