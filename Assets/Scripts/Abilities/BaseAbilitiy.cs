using System;
using UnityEngine;

public abstract class BaseAbilitiy : MonoBehaviour, IAbility
{
    public NameAbility currentAbility;

    //Data
    private SO_NewAbility originalData;
    private SO_NewAbility dataModified;

    private GameObject prefabAbility;
    private FindEnemiesComponent findEnemiesComponent;

    public FindEnemiesComponent FindEnemiesComponent { get => findEnemiesComponent; set => findEnemiesComponent = value; }
    public GameObject PrefabAbility { get => prefabAbility; set => prefabAbility = value; }


    //Init
    public virtual void Initialize(AbilityBaseData data)
    {
        SO_NewAbility newData = (SO_NewAbility)data;
        if (data == null) return;

        originalData = ScriptableObject.Instantiate(newData);
        PrefabAbility = originalData.effect;

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
        if (dataModified == null || dataModified != originalData)
        {
            dataModified = originalData;
        }
    }
    public void SetData(AbilityUpgrades typeData, int newData, bool sum = true)
    {
        if (!Enum.IsDefined(typeof(AbilityUpgrades), typeData))
        {
            Debug.LogError($"Invalid AbilityUpgrade type: {typeData}");
            return;
        }

        int currentValue = GetStatData(typeData);
        SetStatData(typeData, sum ? currentValue + newData : currentValue - newData);
    }


    public AbilityValues GetBasicData()
    {
        return dataModified.basicValues;
    }
    public NameAbility GetTypeAbility()
    {
        return currentAbility;
    }


    public int GetStatData(AbilityUpgrades type)
    {
        return 0;
    }
    public void SetStatData(AbilityUpgrades type, float value)
    {

    }
}