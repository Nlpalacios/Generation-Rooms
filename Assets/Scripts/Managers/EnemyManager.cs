using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Object pool")]
    [SerializeField] private GameObject objectPool;
    [SerializeField] int countEnemies;

    [Header("Enemy")]
    [SerializeField] private List<GameObject> enemiesProperties = new List<GameObject>();
    private Dictionary<typeEnemy, GameObject> keyEnemyType = new Dictionary<typeEnemy, GameObject>();
    private Dictionary<typeEnemy, List<GameObject>> enemyPools = new Dictionary<typeEnemy, List<GameObject>>();

    [Header("Properties - Slime event")]
    [SerializeField] private int numSlimeForUnion;


    //Singleton
    public static EnemyManager Instance { get; private set; }

    //POWER UPS FOR ENEMIES
    public delegate void PowerUpEnemies();
    public event PowerUpEnemies OnSlimePowerUp;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitObjectPool();
        InstantiateEnemy(new Vector3(0, 0, 0), typeEnemy.Slime);
        InstantiateEnemy(new Vector3(0, 0, 0), typeEnemy.Wasp);
    }

    #region Object pool
    private void InitObjectPool()
    {
        foreach (var enemyObject in enemiesProperties) 
        {
            EnemyBasicStates enemyStates = enemyObject.GetComponent<EnemyBasicStates>();
            if (enemyStates == null)
            {
                continue;
            }

            keyEnemyType.Add(enemyStates.parameters.enemyType, enemyObject);
            enemyPools[enemyStates.parameters.enemyType] = new List<GameObject>();

            for (int i = 0; i < countEnemies; i++)
            {
                GameObject currentEnemy = Instantiate(enemyObject, objectPool.transform);
                currentEnemy.name = $"{enemyObject.name}: {i}";
                currentEnemy.SetActive(false);
                enemyPools[enemyStates.parameters.enemyType].Add(currentEnemy);
            }
        }
    }

    public GameObject InstantiateEnemy(Vector3 posStartEnemy, typeEnemy type)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning($"Type enemy {type} not found.");
            return null;
        }

        List<GameObject> pool = enemyPools[type];
        foreach (var enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                EnemyBasicStates enemyScript = enemy.GetComponent<EnemyBasicStates>();
                if (enemyScript == null) return null;

                enemyScript.InitEnemy();
                enemy.SetActive(true);

                Vector3 posInit = new Vector3(posStartEnemy.x, posStartEnemy.y, 0);
                enemy.transform.position = posInit;
                enemy.transform.rotation = new Quaternion(0, 0, 0, 0);
                return enemy;
            }
        }

        return null;
    }

    #endregion


    private void Update()
    {
    }
}
