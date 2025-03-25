using System.Collections.Generic;
using UnityEngine;

public abstract class RoomSettings : MonoBehaviour
{
    [Space, Header("Doors")]
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject DownDoor;
    [SerializeField] private GameObject UpDoor;

    [Space, Header("Player Detection")]
    [SerializeField] private Vector2 sizeBox = new Vector2(20,11);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Colors")]
    [SerializeField] private Color blockedColor = Color.white;
    private Color normalColor = Color.white;

    private List<GameObject> doors = new List<GameObject>();
    private List<GameObject> openDoors = new List<GameObject>();
    [SerializeField] private bool isOpenRoom = true;


    public Vector2 GetArea { get => sizeBox; }
    public bool IsOpenRoom { get => isOpenRoom; set => isOpenRoom = value; }

    private void Awake()
    {
        EventManager.Instance.Subscribe(EnemiesEvents.OnEnableEnemy, StartEnemyDetector);
    }
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(EnemiesEvents.OnEnableEnemy, StartEnemyDetector);
    }

    private void Start()
    {
        InvokeRepeating(nameof(PlayerDetection), 1f, 0.2f);

        doors.Add(LeftDoor);
        doors.Add(RightDoor);
        doors.Add(DownDoor);
        doors.Add(UpDoor);
    }
    private void Update()
    {
        NewUpdate();
    }

    public abstract void NewUpdate();

    //DETECTION ------------------------------
    void PlayerDetection()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, GetArea, 0f, playerLayer);
        if (player == null) return;

        //Debug
        Debug.DrawLine(transform.position, player.transform.position, Color.red);
        GameManager.Instance.TrySaveActualRoom(this);
    }

    public void StartEnemyDetector(object call)
    {
        if (IsInvoking(nameof(EnemyDetector))) return;
        InvokeRepeating(nameof(EnemyDetector), .1f, 2);
    }

    void EnemyDetector()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, GetArea, 0f, enemyLayer);

        if (enemies.Length == 0)
        {
            if (!IsOpenRoom)
            {
                OpenRoom();
                IsOpenRoom = true;
            }

            CancelInvoke(nameof(EnemyDetector));
            return; 
        }

        if (IsOpenRoom)
        {
            CloseRoom();
            IsOpenRoom = false;
        }
    }

    //DOOR MANAGMENT ------------------------
    public void RemoveDoor(directionDoor door)
    {
        switch (door)
        {
            case directionDoor.Left:
                LeftDoor.SetActive(false);
                break;

            case directionDoor.Right:
                RightDoor.SetActive(false);
                break;

            case directionDoor.Up:
                UpDoor.SetActive(false);
                break;

            case directionDoor.Down:
                DownDoor.SetActive(false);
                break;

            default:
                Debug.Log("No se asigno una puerta correcta");
                break;
        }
    }

    public void CloseRoom()
    {
        foreach (GameObject actualDoor in doors)
        {
            if (actualDoor.activeInHierarchy) continue;

            actualDoor.SetActive(true);

            if (actualDoor.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                normalColor = spriteRenderer.color;
                spriteRenderer.color = blockedColor;
            }

            openDoors.Add(actualDoor);
        }
    }

    public void OpenRoom()
    {
        if (openDoors.Count <= 0) return;

        foreach (GameObject actualDoor in openDoors)
        {
            if (actualDoor.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color = normalColor;
            }

            actualDoor.SetActive(false);
        }

        openDoors.Clear();
    }

    //GIZMOS --------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetArea);
    }
    //Max 5
    public void SpawnItem(byte numItems, PlayerWeapon[] types)
    {
        if (numItems <= 0) return;

        float y = 1.2f;
        float spacing = 2.5f; 
        float totalWidth = (numItems - 1) * spacing;
        float startX = -totalWidth / 2f; 

        for (byte i = 0; i < numItems; i++)
        {
            Vector3 pos = new Vector3(startX + (i * spacing), y);
            Vector3 finalPos = pos + transform.position;

            PlayerWeapon weapon = (i < types.Length) ? types[i] : PlayerWeapon.None;
            ItemManager.Instance.SpawnNewMeleeWeapon(finalPos, weapon);
        }
    }
}