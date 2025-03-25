using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Player
    [Header("Player Reference")]
    [SerializeField] private Player playerReference;

    [Header("Current State")]
    [SerializeField] private playerState gameState;

    [Space, Header("Rooms")]
    [SerializeField] private RoomSettings currentRoom;

    [Space, Header("PRINCIPAL PROGRESS --------------")]
    [SerializeField] private int currentLoop = 0;

    //Private variables
    private Camera playerCamera;

    //Singleton
    public static GameManager Instance { get; private set; }

    public RoomSettings GetCurrentRoom { get => currentRoom; }
    public Player GetPlayer => playerReference; 
    public playerState GetCurrentState { get => gameState; set => gameState = value; }

    private List<PlayerItems> ItemsUnlockeds = new List<PlayerItems>();

    //InyectionDependence  || MANAGERS
    private ContainerDependences container;

    //Reference managers
    //private GeneratorRooms generatorRooms;
    private EnemyManager enemyManager;

    #region Star Game
    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameWorldEvents.OnChangeState, SetPlayerState);
        EventManager.Instance.Subscribe(GameWorldEvents.OnFinishLoop, FinishLoop);
    }
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
        EventManager.Instance.TriggerEvent(GameWorldEvents.OnGenerateRooms);
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
    public void TrySaveActualRoom(RoomSettings room)
    {
        if (room == null || room == GetCurrentRoom) return;

        currentRoom = room;
        EventManager.Instance.TriggerEvent(GameWorldEvents.OnChangeRoom, currentRoom.gameObject);
    }

    #endregion

    #region Player State
    public void SetPlayerState(object newState)
    {
        playerState state = (playerState)newState;
        Resolve<IStateManagment>().SetPlayerState(state);
        gameState = state;
    }

    #endregion

    private void FinishLoop(object loop)
    {
        if (!currentRoom.IsOpenRoom) return;

        currentLoop++;

        if (playerReference == null)
        {
            Debug.LogError("NULL PLAYER");
            return;
        }

        playerReference.ResetPosition();
        EventManager.Instance.TriggerEvent(GameWorldEvents.OnUpdateRooms, 1);
    }
}
