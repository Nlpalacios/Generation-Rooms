using System.Collections;
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

    //Reference regeneration rooms
    private GeneratorRooms generatorRooms;

    //Singleton
    public static GameManager Instance { get; private set; }
    public GameObject GetCurrentRoom { get => currentRoom; }
    public Player GetPlayer { get => playerReference; }
    public playerState GetCurrentState { get => gameState; set => gameState = value; }
    

    //InyectionDependence  || MANAGERS
    private ContainerDependences container;

    //EVENTS - DELEGATES --------------------------------------------
    public delegate void CameraMoving(bool isMoving);
    public event CameraMoving OnCameraMoving;

    
    #region Star
    // Start is called before the first frame update
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
        generatorRooms = GeneratorRooms.Instance;
        if (!generatorRooms || !playerReference) 
        {
            Debug.LogError($"INVALID REFERENCE IN {transform.name}");
        }


        Resolve<IStateManagment>().SetPlayerState(playerState.Exploration);
    }

    private void InitializeManagers()
    {
        container = new ContainerDependences();

        // Register types and instances here
        container.Register<IStateManagment, StateManager>();
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

    #region Player

    #endregion
}
