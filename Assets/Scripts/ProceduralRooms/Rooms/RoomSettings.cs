using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomSettings : MonoBehaviour
{
    [Space, Header("Doors")]
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject DownDoor;
    [SerializeField] private GameObject UpDoor;

    [Header("Enemies")]
    [SerializeField] private int maxInitialEnemies = 0;
    [SerializeField] private List<EnemyPercent> enemyPercents;

    [Serializable]
    public class EnemyPercent
    {
        public typeEnemy type;
        public float maxPercent;
    }

    [Space, Header("Player Detection")]
    [SerializeField] private Vector2 sizeBox = new Vector2(20,11);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Colors")]
    [SerializeField] private Color blockedColor = Color.white;
    private Color normalColor = Color.white;

    private List<GameObject> doors = new List<GameObject>();
    private List<GameObject> openDoors = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField] private bool isOpenRoom = true;
    bool playerEnter = false;

    public Vector2 GetArea { get => sizeBox; }
    public bool IsOpenRoom { get => isOpenRoom; set => isOpenRoom = value; }


#if UNITY_EDITOR
    private void OnValidate()
    {
        float total = 0;
        enemyPercents.ForEach(enemy => total += enemy.maxPercent);
        if (total > 100)
        {
            Debug.LogWarning("MUST BE 100");
        }
    }

#endif

    private void Awake()
    {
        EventManager.Instance.Subscribe(EnemiesEvents.OnEnableEnemy, StartEnemyDetector);
    }
    private void Start()
    {
        InvokeRepeating(nameof(PlayerDetection), 1f, 0.2f);

        doors.Add(LeftDoor);
        doors.Add(RightDoor);
        doors.Add(DownDoor);
        doors.Add(UpDoor);
    }


    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(EnemiesEvents.OnEnableEnemy, StartEnemyDetector);
    }
    private void Update()
    {
        NewUpdate();
    }

    public abstract void NewUpdate();
    public abstract void OnPlayerEnter();
    public abstract void OnCloseDoor();
    public abstract void OnOpenDoor();



    //DETECTION ------------------------------
    void PlayerDetection()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, GetArea, 0f, playerLayer);
        if (player == null) 
        { 
            if (playerEnter) playerEnter = false;
            return;
        }

        //Debug
        Debug.DrawLine(transform.position, player.transform.position, Color.red);
        GameManager.Instance.TrySaveActualRoom(this);

        if (!playerEnter){ OnPlayerEnter(); playerEnter = true; }
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

    public void InstantiateAllEnemies()
    {
        if (maxInitialEnemies <= 0) return;
        enemies = EnemyManager.Instance.InstantiateEnemies(maxInitialEnemies, GetArea, (Vector2)transform.position, enemyPercents);
    }
    public void ActiveEnemies()
    {
        if (enemies == null || enemies.Count <= 0) return;

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
                StartEnemyDetector();
            }
        }

        enemies.Clear();
    }
    public void StartEnemyDetector(object call = null)
    {
        if (IsInvoking(nameof(EnemyDetector))) return;
        InvokeRepeating(nameof(EnemyDetector), .1f, 2);
    }

    //DOOR MANAGMENT ------------------------
    #region Door Management  
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
        OnCloseDoor();

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
        OnOpenDoor();

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

    #endregion


    //GIZMOS --------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetArea);
    }

    //Max 5
    public void SpawnItem(byte numItems, PlayerWeapon[] types = null)
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
            PlayerWeapon weapon = PlayerWeapon.None;

            if (types != null)
            {
                weapon = (i < types.Length) ? types[i] : PlayerWeapon.None;
            }

            ItemManager.Instance.SpawnNewMeleeWeapon(finalPos, weapon);
        }
    }
}