using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IHealthCharacterControl
{
    [Header("Movement")]
    [SerializeField][Range(0, 10)] private float playerSpeed = 6;
    [SerializeField][Range(0, 10)] private float maxTotalPlayerSpeed = 9;

    [Header("Health")]
    [SerializeField] private int maxHearts;
    [SerializeField] private int currentHearts;
    [SerializeField] private int maxTotalHearts = 32;

    [Header("Upgrades")]
    [SerializeField] private int playerLevel = 0;
    [SerializeField] private float currentExperience = 0;
    [SerializeField] private float maxExperience = 100;

    [Space]
    [SerializeField] private int currentMaxCards = 2;
    [SerializeField] private int maxTotalCards = 5;

    [Space]
    [SerializeField] private int currentMaxAbilities = 2;
    [SerializeField] private int maxTotalAbilities = 5;

    public static PlayerStats Instance;

    //Player Attributes
    public float PlayerSpeed { get => playerSpeed; set => playerSpeed = value; }

    //Hearts
    public int CurrentHearts { get => currentHearts; set => currentHearts = value; }
    public int GetMaxHearts { get => maxHearts; set => maxHearts = value; }

    //Level
    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }
    public float MaxExperience { get => maxExperience; set => maxExperience = value; }
    public int CurrentMaxCards { get => currentMaxCards; set => currentMaxCards = value; }


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
            DontDestroyOnLoad(gameObject);  
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnChangeExperience, SetExperience);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnChangeExperience, SetExperience);
    }

    #region Hearts
    public void AddHeart(int hearts)
    {
        currentHearts = Mathf.Clamp(currentHearts + hearts, 0, GetMaxHearts);
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
        float difference = 0;

        switch(stats)
        {
            case PlayerBasicStats.speed:
                difference = maxTotalPlayerSpeed - playerSpeed;
                break;
            case PlayerBasicStats.hearts:
                difference = maxTotalHearts - maxHearts;
                break;
            case PlayerBasicStats.totalCards:
                difference = maxTotalCards - currentMaxCards;
                break;
            case PlayerBasicStats.totalUpgrades:
                difference = maxTotalAbilities - currentMaxAbilities;
                break;
        }

        return difference;
    }

    #endregion
}