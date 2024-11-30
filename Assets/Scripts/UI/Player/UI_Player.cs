
using System.Collections.Generic;
using UnityEngine;

public class UI_Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private HealthControl HealthControlPlayer;

    [Header("Hearts")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    List<Heart_State> heart_States = new List<Heart_State>();
    public static UI_Player instance;

    #region Start | Events

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnChangeHealth, HeartManagement);
        InstantiatePlayerHearts();
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnChangeHealth, HeartManagement);
    }
    #endregion

    #region Hearts

    private void InstantiatePlayerHearts()
    {
        if (HealthControlPlayer == null) return;

        //if player has 10 lives, 5 hearts are instantiated
        float totalhearts = HealthControlPlayer.GetMaxHearts / 2;

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
        if (heart_States.Count == 0 || HealthControlPlayer == null) return;

        int totalHearts = HealthControlPlayer.GetCurrentHealth;
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

}
