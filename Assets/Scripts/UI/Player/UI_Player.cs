using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    [Header("Experience")]
    [SerializeField] private Slider sliderExperience;

    [Header("Upgrades - LevelUp")]
    [SerializeField] private UpgradeSelector upgradeSelectorManager;
    [SerializeField] private GameObject upgradeCard;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPanel;

    [Header("Abilities")]
    [SerializeField] private Transform abilityItemTransform;
    [SerializeField] private AbilitySlot abilitySlotPrefab;

    List<GameObject> cardsPooling = new List<GameObject>();
    List<Heart_State> heart_States = new List<Heart_State>();

    List<AbilitySlot> abilitySlots = new List<AbilitySlot>();

    public static UI_Player instance;
    private static PlayerStats stats;

    #region Start | Events

    private void Awake()
    {
        instance = this;
        
        if (sliderExperience == null) return;
        sliderExperience.maxValue = PlayerStats.Instance.MaxExperience;
        InstantiatePoolCards();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnReceiveDamage, HeartManagement);

        EventManager.Instance.Subscribe(PlayerEvents.OnLevelUp, LevelUp);
        EventManager.Instance.Subscribe(PlayerEvents.OnUpdateUI, UpdateAllUI);


        stats = PlayerStats.Instance;
        InstantiatePlayerHearts();
        InitSlots();
    }
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnReceiveDamage, HeartManagement);

        EventManager.Instance.Unsubscribe(PlayerEvents.OnLevelUp, LevelUp);
        EventManager.Instance.Unsubscribe(PlayerEvents.OnUpdateUI, UpdateAllUI);
    }

    #endregion
     
    private void UpdateAllUI(object call)
    {
        if (PlayerStats.Instance.CurrentMaxSlotsAbilities > abilitySlots.Count)
        {
            var newSlots = PlayerStats.Instance.CurrentMaxSlotsAbilities - abilitySlots.Count;

            for (int i = 0; i < newSlots; i++)
            {
                var slot = Instantiate(abilitySlotPrefab, abilityItemTransform);
                abilitySlots.Add(slot);
            }
        }
        if (PlayerStats.Instance.GetMaxHearts > heart_States.Count)
        {
            var newSlots = PlayerStats.Instance.GetMaxHearts - heart_States.Count;
            var totalHearts = heart_States.Count;

            for (int i = 0; i < newSlots; i++)
            {
                GameObject heart = Instantiate(heartPrefab, heartsContainer);

                totalHearts++;
                heart.name = "Heart_" + totalHearts.ToString();

                if (heart.TryGetComponent(out Heart_State heart_State))
                {
                    heart_States.Add(heart_State);
                }
                else
                {
                    Debug.LogError("NULL HEART COMPONENT");
                }
            }
        }

        UpdateSliderExperience();
        HeartManagement();
    }


    #region Hearts
    private void InstantiatePlayerHearts(object call = null)
    {
        if (PlayerStats.Instance == null) return;

        //if player has 10 lives, 5 hearts are instantiated
        float totalhearts = PlayerStats.Instance.GetMaxHearts;

        for (int i = 0; i < (int)totalhearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            heart.name = "Heart_" + i.ToString();

            if (heart.TryGetComponent(out Heart_State heart_State))
            {
                heart_States.Add(heart_State);
            }
            else
            {
                Debug.LogError("NULL HEART COMPONENT");
            }
        }
    }
    public void HeartManagement(object hearts = null)
    {
        if (heart_States.Count == 0 || PlayerStats.Instance == null) return;

        int totalHearts = PlayerStats.Instance.CurrentHearts;
        int maxHearts = heart_States.Count;

        if (totalHearts <= 0)
        {
            heart_States.ForEach(item => { item.SetState(0); });
            return;
        }

        for (int i = 0; i < maxHearts; i++)
        {
            if (i < totalHearts / 2)
            {
                heart_States[i].SetState(2); 
            }
            else if (i == totalHearts / 2 && totalHearts % 2 != 0)
            {
                heart_States[i].SetState(1); 
            }
            else
            {
                heart_States[i].SetState(0); 
            }
        }
    }

    #endregion

    #region Level

    private void LevelUp(object level)
    {
        UpdateSliderExperience();
        InstantiateUpgrades();
    }
    private void UpdateSliderExperience()
    {
        sliderExperience.value = stats.CurrentExperience;
    }

    private void InstantiatePoolCards()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject card = Instantiate(upgradeCard.gameObject, cardContainer);
            card.SetActive(false);
            cardsPooling.Add(card);
        }
    }
    private void InstantiateUpgrades()
    {
        if (upgradeCard == null || cardsPooling.Count == 0) return;
        GameManager.Instance.SetPlayerState(playerState.Inspection);
        cardPanel.SetActive(true);
        int maxCards = PlayerStats.Instance.CurrentMaxCards;

        for (int i = 0; i < maxCards; i++)
        {
            if (i > cardsPooling.Count) break;

            CardUpgrade card = cardsPooling[i].gameObject.GetComponent<CardUpgrade>();
            card.selectButton.onClick.AddListener(() => { CardSelected(card); });
            card.gameObject.SetActive(true);

            AbilityBasicData data = upgradeSelectorManager.GenerateData();
            if (data == null) { Debug.LogWarning("NO AVALIABLE DATA"); continue; }
            card.UpdateInfo(data);
        }

        upgradeSelectorManager.ResetPreferences();
    }

    private void CardSelected(CardUpgrade card)
    {
        AbilityBasicData data = card.UpgradeData;
        if (data == null) { Debug.LogWarning("NO AVALIABLE DATA"); return; }

        if (data.GetBasicData().automaticUse)
        {
            AbilityController.Instance.StartPlayerAbility(data);
            ResumeGame();
            return;
        }

        AbilitySlot currentSlot = GetEmptySlot();
        if (currentSlot == null || currentSlot.ContainData()) { Debug.LogWarning("NO AVALIABLE SLOTS"); return; }

        //UNLOCK ABILITY
        if (data.upgradeType == UpgradeType.NewAbility)
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnUnlockAbility, data.newAbilityData.type);
        }

        currentSlot.ResetSlot(data);
        ResumeGame();
    }
    private void ResumeGame()
    {
        //Continue game
        cardPanel.SetActive(false);
        GameManager.Instance.SetPlayerState(playerState.Exploration);
    }

    #endregion

    #region Abilities
    private void InitSlots()
    {
        if (PlayerStats.Instance == null) return;
        int totalSlots = PlayerStats.Instance.CurrentMaxSlotsAbilities;

        foreach (var slot in abilitySlots)
        {
            Destroy(slot.gameObject);
        }

        abilitySlots.Clear();

        for (int i = 0;i < totalSlots; i++)
        {
            var slot = Instantiate(abilitySlotPrefab, abilityItemTransform);
            abilitySlots.Add(slot);
        }
    }
    private AbilitySlot GetEmptySlot()
    {
        return abilitySlots.FirstOrDefault(slot => !slot.ContainData());
    }

    #endregion
}