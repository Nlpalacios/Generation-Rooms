using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUpgrade : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI tmp_name;
    [SerializeField] private TextMeshProUGUI tmp_typeCard;
    [SerializeField] private TextMeshProUGUI tmp_Description;

    [Header("Icon")]
    [SerializeField] private Image icon;

    public void UpdateInfo(UpgradeData data)
    {
        if (data == null) return;

        if (data.upgradeType == UpgradeType.playerUpgrade) 
        {
            //Set card icon
            icon.sprite = data.icon;

            //Set card texts
            tmp_name.text = UpgradesName.NewUpgradeName;
            tmp_typeCard.text = UpgradesName.NewUpgrade;
            tmp_Description.text = $"+{data.stat} {data.playerUpgrades.ToString().ToLower()}";
        }

        else if (data.upgradeType == UpgradeType.NewAbility) 
        {
            Debug.Log("NO ABILITY");
        }
        else if (data.upgradeType == UpgradeType.AbilityUpgrade)
        {
            Debug.Log("NO ABILITY");
        }
    }
}
