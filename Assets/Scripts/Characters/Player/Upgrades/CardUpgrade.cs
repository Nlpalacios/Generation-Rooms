using System;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Button")]
    public Button selectButton;

    private AbilityBasicData upgradeData;
    public AbilityBasicData UpgradeData  => upgradeData;

    private void OnDisable()
    {
        tmp_name.text = "NULL CARD";
        tmp_typeCard.text = "NULL";

        upgradeData = null;
    }

    public void UpdateInfo(AbilityBasicData data)
    {
        upgradeData = data;
        if (upgradeData == null) { Debug.Log("data null"); return; }

        //Set card icon
        icon.sprite = data.GetBasicData().icon;
        Debug.Log(data.GetBasicData().name);

        if (data.upgradeType == UpgradeType.playerUpgrade)
        {
            SO_PlayerAbility dataAbility = data.playerAbilityData;

            //Set card texts
            tmp_name.text = UpgradesName.NewUpgradeName;
            tmp_typeCard.text = UpgradesName.NewUpgrade;

            if (string.IsNullOrEmpty(data.playerAbilityData.description))
            {
                tmp_Description.text = $"+{(int)dataAbility.upgradeValue} {dataAbility.type.ToString().ToLower()}";
            }
            else
                tmp_Description.text = dataAbility.description;

            return;
        }

        SO_NewAbility ability = data.newAbilityData;

        if (data.upgradeType == UpgradeType.NewAbility)
        {
            tmp_typeCard.text = UpgradesName.NewAbility;
            tmp_name.text = ability.abilityName.ToString().ToLower();

            tmp_Description.text = string.Format(
             "{0}{1}\n{2}{3}\n{4}{5}\n{6}{7}",
             UpgradesName.damage, ability.basicValues.damage,
             UpgradesName.range, ability.basicValues.range,
             UpgradesName.delay, ability.basicValues.delay,
             UpgradesName.exp, ability.basicValues.expCost
             );

            return;
        }

        tmp_typeCard.text = UpgradesName.NewAbilityUpgrade;

        Debug.Log(data.GetBasicData().name);
        tmp_name.text = string.IsNullOrEmpty(data.GetBasicData().name) ? data.GetBasicData().abilityType.ToString().ToLower() :
                                                          data.GetBasicData().name.ToLower();

        float damageValue = ability.basicValues.damage;
        float rangeValue = ability.basicValues.range;
        float expValue = ability.basicValues.expCost;
        float delayValue = ability.basicValues.delay;

        float upgradeValue = 0f;
        upgradeValue = data.upgradeData.upgradeValue;

        switch (data.upgradeData.typeUpgrade)
        {
            case AbilityUpgrades.damage:
                damageValue += upgradeValue;
                break;

            case AbilityUpgrades.range:
                rangeValue += upgradeValue;
                break;

            case AbilityUpgrades.exp:
                expValue += upgradeValue;
                break;

            case AbilityUpgrades.delay:
                upgradeValue = - data.upgradeData.upgradeValue;
                delayValue += upgradeValue;
                break;
        }

        if (!string.IsNullOrEmpty(data.GetBasicData().description))
        {
            tmp_Description.text = data.GetBasicData().description.ToLower();
            return;
        }

        AbilityUpgrades typeUpgrade = data.upgradeData.typeUpgrade;

        tmp_Description.text = string.Format("{0}{1}{8}\n{2}{3}{9}\n{4}{5}s{10}\n{6}{7}{11}",
         UpgradesName.damage, ability.basicValues.damage,
         UpgradesName.range, ability.basicValues.range,
         UpgradesName.delay, ability.basicValues.delay,
         UpgradesName.exp, ability.basicValues.expCost,


          typeUpgrade == AbilityUpgrades.damage && upgradeValue != 0 ? $" {upgradeValue:+0.##}" : "",
          typeUpgrade == AbilityUpgrades.range && upgradeValue != 0 ? $" {upgradeValue:+0.##}" : "",
          typeUpgrade == AbilityUpgrades.delay && upgradeValue != 0 ? $" {-Math.Abs(upgradeValue):0.##s}" : "",
          typeUpgrade == AbilityUpgrades.exp && upgradeValue != 0 ? $" {-Math.Abs(upgradeValue):0.##}" : "");

    }
}
