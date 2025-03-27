using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private AbilityDatabase abilityDatabase;
    private Dictionary<NameAbility, IAbility> activeAbilities = new Dictionary<NameAbility, IAbility>();

    public Dictionary<NameAbility, IAbility> CurrentAbilities { get => activeAbilities; set => activeAbilities = value; }
    public static AbilityController Instance { get; private set; }
    public AbilityDatabase database { get => abilityDatabase; set => abilityDatabase = value; }

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        EventManager.Instance.Subscribe(CombatEvents.OnUnlockAbility, AddAbility);
        EventManager.Instance.Subscribe(CombatEvents.OnStartPlayerAbility, StartPlayerAbility);
        EventManager.Instance.Subscribe(CombatEvents.OnStartAbility, StartAbility);
    }
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(CombatEvents.OnUnlockAbility, AddAbility);
        EventManager.Instance.Unsubscribe(CombatEvents.OnStartPlayerAbility, StartPlayerAbility);
        EventManager.Instance.Unsubscribe(CombatEvents.OnStartAbility, StartAbility);
    }

    public void AddAbility(object call)
    {
        NameAbility ability = (NameAbility)call;
        AbilityBaseData data = abilityDatabase.GetAbilityData(ability);
        if (data == null || activeAbilities.ContainsKey(ability)) { Debug.LogError("DATA NULL"); return; }

        IAbility newAbility = CreateAbility(ability);
        newAbility.Initialize(data);

        activeAbilities.Add(ability, newAbility);
        Debug.LogWarning("ABILITY ADDED: " + ability);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="call">TYPE ABILITY - NAME</param>
    public void StartAbility(object call)
    {
        AbilityBasicData type = (AbilityBasicData)call;
        if (call == null) return;

        IAbility ability = GetAbility(type.newAbilityData.type);
        if (ability != null)
        {
            ability.ActivateAbility();
        }
        else
        {
            Debug.LogError($"Ability: -{type}- not find in actives");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="call">TYPE ABILITY - PLAYER UPGRADE</param>
    public void StartPlayerAbility(object call)
    {
        AbilityBasicData abilityUpgradeData = (AbilityBasicData)call;
        if (abilityUpgradeData == null) return;

        EventManager.Instance.TriggerEvent(PlayerEvents.OnReceiveUpgrade, abilityUpgradeData);
    }


    public void StopAbility(NameAbility type)
    {
        IAbility ability = GetAbility(type);
        if (ability != null)
        {
            ability.DeactivateAbility();
        }
    }
    private IAbility CreateAbility(NameAbility type)
    {
        switch (type)
        {
            case NameAbility.Ability_Ray: return new Ability_Ray();
            case NameAbility.Ability_Rock: return new Ability_Rocks();
            default: return null;
        }
    }
    public IAbility GetAbility(NameAbility type)
    {
        activeAbilities.TryGetValue(type, out IAbility ability);
        return ability;
    }


    public bool HasAbilities()
    {
        return CurrentAbilities.Count > 0;
    }
    public bool HasAbility(NameAbility type)
    {
        return CurrentAbilities.ContainsKey(type);
    }


    public List<NameAbility> GetCurrentAbilities()
    {
        return CurrentAbilities.Keys.ToList();
    }
}