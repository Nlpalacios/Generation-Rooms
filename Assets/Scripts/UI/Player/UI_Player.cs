using System.Collections.Generic;
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

    [Header("Current Level")]
    [SerializeField] private float currentExperience = 0;

    List<GameObject> cardsPooling = new List<GameObject>();
    List<Heart_State> heart_States = new List<Heart_State>();
    public static UI_Player instance;

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
        EventManager.Instance.Subscribe(PlayerEvents.OnChangeExperience, SetExperience);
        EventManager.Instance.Subscribe(PlayerEvents.OnLevelUp, LevelUp);
        InstantiatePlayerHearts();
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnReceiveDamage, HeartManagement);
        EventManager.Instance.Unsubscribe(PlayerEvents.OnChangeExperience, SetExperience);
        EventManager.Instance.Unsubscribe(PlayerEvents.OnLevelUp, LevelUp);
    }

    #endregion

    #region Hearts

    private void InstantiatePlayerHearts()
    {
        if (PlayerStats.Instance == null) return;

        //if player has 10 lives, 5 hearts are instantiated
        float totalhearts = PlayerStats.Instance.GetMaxHearts / 2;

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

    public void HeartManagement(object hearts)
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

    private void SetExperience(object exp)
    {
        currentExperience += (float)exp;
        UpdateSliderExperience();
    }

    private void LevelUp(object level)
    {
        currentExperience = 0;

        UpdateSliderExperience();
        InstantiateUpgrades();
    }

    private void UpdateSliderExperience()
    {
        sliderExperience.value = currentExperience;
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
            card.gameObject.SetActive(true);

            UpgradeData data = upgradeSelectorManager.GenerateData();
            if (data == null) { Debug.LogError("NO AVALIABLE DATA"); continue; }
            card.UpdateInfo(data);
        }
    }

    #endregion
}
