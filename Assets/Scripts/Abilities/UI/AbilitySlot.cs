using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    [SerializeField] private Image icon; 
    private AbilityBasicData upgradeData = null;

    public bool ContainData() {  return upgradeData != null; }

    public void ResetSlot(AbilityBasicData newData = null)
    {
        if (newData != null)
        {
            upgradeData = newData;
        }

        if (upgradeData != null)
        {
            icon.sprite = upgradeData.GetBasicData().icon;
        }
    }

    public void OnSelect()
    {
        if (upgradeData == null || upgradeData.upgradeType == UpgradeType.None) { Debug.Log("NULL DATA"); return; }
        if (!PlayerStats.Instance.UseAbility(GetExp())){ Debug.LogWarning("NO EXP FOR USE"); return; }

        if (upgradeData.upgradeType == UpgradeType.playerUpgrade)
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnStartPlayerAbility, upgradeData);
        }
        else
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnStartAbility, upgradeData);
        }
    }

    int GetExp()
    {
        AbilityBaseData data = upgradeData.GetBasicData();

        if (data is SO_NewAbility)
        {
            SO_NewAbility abilityData = (SO_NewAbility)data;
            return abilityData.basicValues.expCost;
        }
        else if (data is SO_PlayerAbility)
        {
            SO_PlayerAbility playerData = (SO_PlayerAbility)data;
            return playerData.basicValues.expCost;
        }

        Debug.LogWarning("NO DATA");
        return 0;
    }
}