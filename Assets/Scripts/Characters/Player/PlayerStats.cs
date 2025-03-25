using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class PlayerStats : MonoBehaviour, IHealthCharacterControl
{
    [Header("Movement")]
    [SerializeField][UnityEngine.Range(0, 10)] private float playerSpeed = 6;

    [Header("Health")]
    [SerializeField] private int currentHearts;
    [SerializeField] private int maxHearts;

    [Header("Upgrades")]
    [SerializeField] private int playerLevel = 0;
    [SerializeField] private float currentExperience = 0;
    [SerializeField] private float initialMaxExperience = 100;

    [Space]
    [SerializeField] private int currentMaxCards = 2;
    [SerializeField] private int maxTotalCards = 5;

    [Space]
    [SerializeField] private int currentMaxSlotsAbilities = 2;
    [SerializeField] private int maxSlotsAbilities = 5;

    public static PlayerStats Instance;

    //Player Attributes
    public float PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }

    //Hearts
    public int CurrentHearts { get => currentHearts; set => currentHearts = value; }
    public int GetMaxHearts { get => maxHearts; set => maxHearts = value; }

    //Level
    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }
    public float MaxExperience { get => initialMaxExperience; set => initialMaxExperience = value; }
    public int CurrentMaxCards { get => currentMaxCards; set => currentMaxCards = value; }
    public int CurrentMaxSlotsAbilities { get => currentMaxSlotsAbilities; set => currentMaxSlotsAbilities = value; }

    public Action OnPlayerDeath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);  
        }
    }
    private void OnEnable()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnChangeExperience, SetExperience);
        EventManager.Instance.Subscribe(PlayerEvents.OnReceiveUpgrade, UpgradePlayer);
        EventManager.Instance.Subscribe(PlayerEvents.OnReceiveDamage, HeartManagement => { RemoveHearts((int)HeartManagement); } );
    }
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnChangeExperience, SetExperience);
        EventManager.Instance.Unsubscribe(PlayerEvents.OnReceiveDamage, HeartManagement => { RemoveHearts((int)HeartManagement); });
    }

    public void ResetData()
    {
        currentHearts = maxHearts;
    }
    public void UpgradePlayer(object call)
    {
        var upgradeValues = (AbilityBasicData)call;
        if (upgradeValues == null) { Debug.LogError("CORRUPTED OR NULL DATA");  return; }

        var value = upgradeValues.valueUpgrade;

        switch (upgradeValues.playerUpgrades)
        {
            case PlayerBasicStats.None:
                break;

            case PlayerBasicStats.speed:

                //if (upgradeValues.duration != 0)
                //{
                //    StartCoroutine(TemporalSpeedUpgrade(upgradeValues.valueUpgrade, upgradeValues.duration));
                //}
                //else
                    playerSpeed += value;
                break;

            case PlayerBasicStats.hearts:

                if (upgradeValues.singleUse)
                {
                    ModifyHearts((int)value, upgradeValues.restoreHearts);
                }
                else
                {
                    AddHeart((int)value);
                }

                break;

            case PlayerBasicStats.totalCards:
                currentMaxCards += (int)value;
                break;

            case PlayerBasicStats.totalUpgrades:
                currentMaxSlotsAbilities += (int)value;
                break;
        }

        UpdateUI();
    }

    public IEnumerator TemporalSpeedUpgrade(float value, float duration)
    {
        var actualValue = playerSpeed;
        playerSpeed += value;

        yield return new WaitForSeconds(duration);

        playerSpeed = actualValue;
    }

    #region Hearts
    public void AddHeart(int hearts)
    {
        currentHearts += hearts;
        UpdateUI();
    }
    public void ModifyHearts(int amount, bool restoreAll = false)
    {
        if (restoreAll)
        {
            currentHearts = (maxHearts * 2);
            return;
        }

        maxHearts += amount;
        currentHearts += (amount * 2);
    }
    public void RemoveHearts(int damage)
    {
        currentHearts = Mathf.Max(currentHearts - damage, 0);
        TakeDamage(); 

        if (currentHearts <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public void TakeDamage()
    {
        //StartCoroutine(takeDamageAnimation());
    }
    IEnumerator takeDamageAnimation()
    {
        yield return null;
    }

    #endregion

    #region Experience
    private void SetExperience(object exp)
    {
        currentExperience += (float)exp;

        if (currentExperience >= MaxExperience)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        currentExperience = 0;
        playerLevel++;

        EventManager.Instance.TriggerEvent(PlayerEvents.OnLevelUp, playerLevel);
    }

    #endregion

    #region Getters
    public float GetDifferenceStats(PlayerBasicStats stats)
    {
        switch (stats)
        {
            case PlayerBasicStats.speed:
                return 1;

            case PlayerBasicStats.hearts:
                return 1;

            case PlayerBasicStats.totalCards:
                return maxTotalCards - currentMaxCards;

            case PlayerBasicStats.totalUpgrades:
                return maxSlotsAbilities - CurrentMaxSlotsAbilities;

            default:
                return 0;
        }
    }
    private bool PosibleUpgrade(PlayerBasicStats statUpgrade)
    {
        return GetDifferenceStats(statUpgrade) > 0;
    }
    public (PlayerBasicStats, float) GetRandomPlayerUpgrade()
    {
        var upgrades = new List<(PlayerBasicStats stat, float min, float max, float value)>
        {
          (PlayerBasicStats.totalCards, 0f, 0.1f, 1),
          (PlayerBasicStats.totalUpgrades, 0.1f, 0.3f, 1),

          (PlayerBasicStats.speed, 0.3f, 0.6f, Random.Range(0, 0.2f)),
          (PlayerBasicStats.hearts, 0.6f, 1f, 1)
        };

        var possibleUpgrades = upgrades.Where(upgrade => PosibleUpgrade(upgrade.stat)).ToList();
        if (possibleUpgrades.Count == 0)
        {
            return (PlayerBasicStats.None, 0f);
        }

        float roll = Random.value;

        foreach (var upgrade in upgrades)
        {
            if (roll >= upgrade.min && roll < upgrade.max && PosibleUpgrade(upgrade.stat))
            {
                return (upgrade.stat, upgrade.value);
            }
        }

        return (PlayerBasicStats.None, 0f);
    }

    #endregion

    private void UpdateUI()
    {
        EventManager.Instance.TriggerEvent(PlayerEvents.OnUpdateUI);
    }
}