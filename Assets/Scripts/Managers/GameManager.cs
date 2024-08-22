using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Player
    [Header("Player Reference")]
    [SerializeField] private Player playerReference;

    [Header("Current State")]
    [SerializeField] private playerState gameState;

    [Space, Header("Rooms")]
    [SerializeField] private GameObject currentRoom;

    //Private variables
    private Camera playerCamera;

    //Singleton
    public static GameManager Instance { get; private set; }

    //
    public GameObject GetCurrentRoom { get => currentRoom; }
    public Player GetPlayer => playerReference; 
    public playerState GetCurrentState { get => gameState; set => gameState = value; }
    

    //InyectionDependence  || MANAGERS
    private ContainerDependences container;

    //Reference managers
    private GeneratorRooms generatorRooms;
    private EnemyManager enemyManager;

    //EVENTS - DELEGATES --------------------------------------------
    public Action<bool> OnCameraMoving;

    
    #region Star Game

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        InitSingletons();
        ValidateReferences();

        SetPlayerState(playerState.Exploration);
    }

    private void ValidateReferences()
    {
        if (!playerReference)
        {
            Debug.LogError($"INVALID PLAYER IN GAME MANAGER");
        }

        if (!playerCamera)
        {
            Debug.LogError($"INVALID CAMERA IN GAME MANAGER");
        }
    }

    #endregion

    #region Dependences and Managers

    private void InitSingletons()
    {
        var instanceTypes = new List<Type> { typeof(GeneratorRooms),
                                             typeof(EnemyManager)
        };

        List<GameObject> instances = new List<GameObject>();

        foreach (var type in instanceTypes)
        {
            var instance = (MonoBehaviour)type.GetProperty("Instance").GetValue(null, null);
            instances.Add(instance.gameObject);
        }

        if (instances.Any(n => n == null))
        {
            Debug.LogError("INSTANCE ERROR");
        }
    }
    private void InitializeManagers()
    {
        container = new ContainerDependences();

        container.Register<IStateManagment, StateManager>();
        // Register MORE types and instances here

    }
    public T Resolve<T>()
    {
        return container.Resolve<T>();
    }

    #endregion

    #region Rooms

    // Actual Room
    public void SaveActualRoom(GameObject room)
    {
        if (room != GetCurrentRoom)
        {
            currentRoom = room;
        }
    }


    //EVENTS OF CAMERA -----------------------------
    public void EventCameraMoving(bool moving = true)
    {
        OnCameraMoving?.Invoke(moving);
    }

    #endregion

    #region Player State

    public void SetPlayerState(playerState newState)
    {
        Resolve<IStateManagment>().SetPlayerState(newState);
        gameState = newState;
    }

    #endregion
}
