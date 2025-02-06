using System;
using UnityEngine;

public abstract class BaseAbilitiy : MonoBehaviour, IAbility
{
    public TypeAbility currentAbility;

    //Data
    private SO_Ability initialData;
    private SO_Ability dataModified;

    private GameObject prefabAbility;
    private FindEnemiesComponent findEnemiesComponent;

    public FindEnemiesComponent FindEnemiesComponent { get => findEnemiesComponent; set => findEnemiesComponent = value; }
    public GameObject PrefabAbility { get => prefabAbility; set => prefabAbility = value; }


    //Init
    public virtual void Initialize(SO_Ability data)
    {
        initialData = ScriptableObject.Instantiate(data);
        PrefabAbility = initialData.effect;

        if (PrefabAbility == null) { Debug.LogError("No effect in DATA"); return; }
        ResetData(null);

        //Suscribe event
        EventManager.Instance.Unsubscribe(PlayerEvents.OnDeath, ResetData); 
        EventManager.Instance.Subscribe(PlayerEvents.OnDeath, ResetData);
    }

    //Components
    public void InitializeFindEnemiesComponent()
    {
        FindEnemiesComponent = new FindEnemiesComponent();
    }


    //Abstract functions
    public abstract void ActivateAbility();
    public abstract void DeactivateAbility();
    public abstract bool IsAbilityActive();
    public abstract void UpgradeAbility();

    //Data Management
    public virtual void ResetData(object call)
    {
        if (dataModified == null || dataModified != initialData)
        {
            dataModified = initialData;
        }
    }
    public void SetData(AbilityUpgrades typeData, int newData, bool sum = true)
    {
        if (!Enum.IsDefined(typeof(AbilityUpgrades), typeData))
        {
            Debug.LogError($"Invalid AbilityUpgrade type: {typeData}");
            return;
        }

        int currentValue = dataModified.GetStat(typeData);
        dataModified.SetStat(typeData, sum ? currentValue + newData : currentValue - newData);
    }
    public SO_Ability GetData()
    {
        return dataModified;
    }
    public TypeAbility GetTypeAbility()
    {
        return currentAbility;
    }
}
