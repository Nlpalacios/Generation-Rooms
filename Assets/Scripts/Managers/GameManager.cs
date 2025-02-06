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

    //Private variables
    private Camera playerCamera;

    //Singleton
    public static GameManager Instance { get; private set; }

    public RoomSettings GetCurrentRoom { get => currentRoom; }
    public Player GetPlayer => playerReference; 
    public playerState GetCurrentState { get => gameState; set => gameState = value; }

    private List<ItemsToUnlock> ItemsUnlockeds = new List<ItemsToUnlock>();

    //InyectionDependence  || MANAGERS
    private ContainerDependences container;

    //Reference managers
    //private GeneratorRooms generatorRooms;
    private EnemyManager enemyManager;

    // Change to future //
    //Relation between the new item and its corresponding weapon
    private Dictionary<ItemsToUnlock, PlayerWeapon> itemToWeaponMap = new Dictionary<ItemsToUnlock, PlayerWeapon>(){
    { ItemsToUnlock.Unlock_Sword, PlayerWeapon.Weapon_Sword },
    { ItemsToUnlock.Unlock_Hammer, PlayerWeapon.Weapon_Hammer },
    { ItemsToUnlock.Unlock_Axe, PlayerWeapon.Weapon_Axe },
    { ItemsToUnlock.Unlock_Boomerang, PlayerWeapon.Weapon_Boomerang }

    };

    // //

    #region Star Game

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameWorldEvents.OnChangeState, SetPlayerState);
        EventManager.Instance.Subscribe(PlayerEvents.OnUnlockItem, UnlockItem);
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

    private void InstatiatePlayer()
    {
        playerReference = Instantiate(playerReference);
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


    private void UnlockItem(object item)
    {
        NewItem currentItem = (NewItem)item;
        if (currentItem == null) return;

        ItemsUnlockeds.Add(currentItem.item);

        if (itemToWeaponMap.TryGetValue(currentItem.item, out PlayerWeapon weapon))
        {
            EventManager.Instance.TriggerEvent(CombatEvents.OnChangeWeapon, weapon);
        }
    }
}
