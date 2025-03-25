using System;
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
        icon.sprite = data.icon;
        Debug.Log(data.name);

        if (data.upgradeType == UpgradeType.playerUpgrade)
        {
            //Set card texts
            tmp_name.text = UpgradesName.NewUpgradeName;
            tmp_typeCard.text = UpgradesName.NewUpgrade;

            if (string.IsNullOrEmpty(data.description))
            {
                tmp_Description.text = $"+{(int)data.valueUpgrade} {data.playerUpgrades.ToString().ToLower()}";
            }
            else
                tmp_Description.text = data.description;

            return;
        }

        SO_NewAbility ability = AbilityController.Instance.database.GetAbilityData(data.typeAbility);

        if (data.upgradeType == UpgradeType.NewAbility)
        {
            tmp_typeCard.text = UpgradesName.NewAbility;
            tmp_name.text = data.typeAbility.ToString().ToLower();

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

        Debug.Log(data.name);
        tmp_name.text = string.IsNullOrEmpty(data.name) ? data.typeAbility.ToString().ToLower():
                                                          data.name.ToLower();

        float damageValue = ability.basicValues.damage;
        float rangeValue = ability.basicValues.range;
        float expValue = ability.basicValues.expCost;
        float delayValue = ability.basicValues.delay;

        float upgradeValue = 0f;

        switch (data.abilityUpgrades)
        {
            case AbilityUpgrades.damage:
                upgradeValue = data.valueUpgrade;
                damageValue += upgradeValue;
                break;

            case AbilityUpgrades.range:
                upgradeValue = data.valueUpgrade;
                rangeValue += upgradeValue;
                break;

            case AbilityUpgrades.exp:
                upgradeValue = data.valueUpgrade;
                expValue += upgradeValue;
                break;

            case AbilityUpgrades.delay:
                upgradeValue = -data.valueUpgrade;
                delayValue += upgradeValue;
                break;
        }

        if (!string.IsNullOrEmpty(data.description))
        {
            tmp_Description.text = data.description.ToLower();
            return;
        }

        tmp_Description.text = string.Format("{0}{1}{8}\n{2}{3}{9}\n{4}{5}s{10}\n{6}{7}{11}",
         UpgradesName.damage, ability.basicValues.damage,
         UpgradesName.range, ability.basicValues.range,
         UpgradesName.delay, ability.basicValues.delay,
         UpgradesName.exp, ability.basicValues.expCost,

          data.abilityUpgrades == AbilityUpgrades.damage && upgradeValue != 0 ? $" {upgradeValue:+0.##}" : "",
          data.abilityUpgrades == AbilityUpgrades.range && upgradeValue != 0 ? $" {upgradeValue:+0.##}" : "",
          data.abilityUpgrades == AbilityUpgrades.delay && upgradeValue != 0 ? $" {-Math.Abs(upgradeValue):0.##s}" : "",
          data.abilityUpgrades == AbilityUpgrades.exp && upgradeValue != 0 ? $" {-Math.Abs(upgradeValue):0.##}" : "");
    }
}
