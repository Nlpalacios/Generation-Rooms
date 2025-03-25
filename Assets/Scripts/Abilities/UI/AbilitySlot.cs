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
            icon.sprite = upgradeData.icon;
        }
    }

    public void OnSelect()
    {
        if (upgradeData == null || upgradeData.upgradeType == UpgradeType.None) { Debug.Log("NULL DATA"); return; }

        if (upgradeData.upgradeType == UpgradeType.playerUpgrade)
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnStartPlayerAbility, upgradeData);
        }
        else
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnStartAbility, upgradeData);
        }
    }
}