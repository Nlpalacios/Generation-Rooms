using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RoomSettings;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("Object pool")]
    [SerializeField] private GameObject objectPool;
    [SerializeField] int countEnemies;

    [Header("Enemy - Path prefabs")]
    [SerializeField] private string pathEnemies = "";
    [SerializeField] private List<GameObject> enemiesProperties = new List<GameObject>();
    private Dictionary<typeEnemy, GameObject> keyEnemyType = new Dictionary<typeEnemy, GameObject>();
    private Dictionary<typeEnemy, List<GameObject>> enemyPools = new Dictionary<typeEnemy, List<GameObject>>();

    private HashSet<GameObject> activeEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> enemiesAbilitySelected = new HashSet<GameObject>();
    private HashSet<GameObject> enemiesRoomsSelected = new HashSet<GameObject>();

    //Singleton
    public static EnemyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        //LoadEnemiesPrefab();
        InitObjectPool();
    }

    #region Object pool

    [ContextMenu("Load enemies")]
    public void LoadEnemiesPrefab()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { pathEnemies });
        List<string> paths = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject enemy = AssetDatabase.LoadAssetAtPath<GameObject>(path); ;
            if (enemy == null) continue;

            enemiesProperties.Add(enemy);
            paths.Add(path);
        }
    }
    private void InitObjectPool()
    {
        foreach (var enemyObject in enemiesProperties) 
        {
            EnemyBasicStates enemyStates = enemyObject.GetComponent<EnemyBasicStates>();
            if (enemyStates == null) continue;
            
            keyEnemyType.Add(enemyStates.parameters.enemyType, enemyObject);
            enemyPools[enemyStates.parameters.enemyType] = new List<GameObject>();

            for (int i = 0; i < countEnemies; i++)
            {
                GameObject currentEnemy = Instantiate(enemyObject, objectPool.transform);
                currentEnemy.name = $"{i}_{enemyObject.name}";
                enemyPools[enemyStates.parameters.enemyType].Add(currentEnemy);
                currentEnemy.SetActive(false);
            }
        }
    }

    public List<GameObject> InstantiateEnemies(int totalEnemies, Vector2 spawnArea, Vector2 roomPos, List<EnemyPercent> enemyPercent, bool spawnEnable = false)
    {
        if (enemyPercent.Count <= 0 || totalEnemies <= 0) return null;
        List<GameObject> enemies = new List<GameObject>();
        int remainingEnemies = totalEnemies;
        foreach (var enemy in enemyPercent)
        {
            if (enemy.maxPercent <= 0) continue;
            int total = Mathf.Min((int)((totalEnemies * enemy.maxPercent) / 100), remainingEnemies);
            remainingEnemies -= total;

            for (int i = 0; i < total; i++)
            {
                float x = Random.Range(roomPos.x - spawnArea.x / 2, roomPos.x + spawnArea.x / 2);
                float y = Random.Range(roomPos.y - spawnArea.y / 2, roomPos.y + spawnArea.y / 2);
                Vector3 finalPos = new Vector3(x, y, 0);

                GameObject newEnemy = InstantiateEnemy(finalPos, enemy.type, spawnEnable, true);
                if (newEnemy != null)
                {
                    enemies.Add(newEnemy);
                    enemiesRoomsSelected.Add(newEnemy);
                }
            }
        }

        return enemies;
    }
    public GameObject InstantiateEnemy(Vector3 posStartEnemy, typeEnemy type, bool spawnEnable = true, bool selectedRooms = false)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogWarning($"Type enemy {type} not found.");
            return null;
        }

        List<GameObject> pool = enemyPools[type];

        foreach (var enemy in pool)
        {
            if (selectedRooms)
            {
                if (!enemy.activeInHierarchy && !enemiesRoomsSelected.Contains(enemy))
                {
                    return ActivateEnemy(enemy, posStartEnemy, spawnEnable);
                }
            }

            else if (!enemy.activeInHierarchy)
            {
                return ActivateEnemy(enemy, posStartEnemy, spawnEnable);
            }
        }

        GameObject newEnemy = Instantiate(keyEnemyType[type], objectPool.transform);
        enemyPools[type].Add(newEnemy); 

        return ActivateEnemy(newEnemy, posStartEnemy, spawnEnable);
    }
    private GameObject ActivateEnemy(GameObject enemy, Vector3 position, bool spawnEnable)
    {
        EnemyBasicStates enemyScript = enemy.GetComponent<EnemyBasicStates>();
        if (enemyScript == null) return null;

        enemyScript.InitEnemy();
        enemy.SetActive(spawnEnable);

        Vector3 posInit = new Vector3(position.x, position.y, 0);
        enemy.transform.localPosition = posInit;
        enemy.transform.rotation = Quaternion.identity;

        EventManager.Instance.TriggerEvent(EnemiesEvents.OnEnableEnemy);
        UpdateEnemyState(enemy, true);
        return enemy;
    }

    #endregion
    public void UpdateEnemyState(GameObject enemy, bool isActive)
    {
        if (isActive)
        {
            activeEnemies.Add(enemy);
        }
        else
        {
            activeEnemies.Remove(enemy);
            enemiesAbilitySelected.Remove(enemy);
        }
    }

    public int GetCountActiveEnemies()
    {
        return activeEnemies.Count;
    }
    public GameObject GetDiferentActiveEnemy()
    {
        foreach (var enemy in activeEnemies)
        {
            if (!enemiesAbilitySelected.Contains(enemy))
            {
                enemiesAbilitySelected.Add(enemy);
                return enemy;
            }
        }

        return null;
    }
    public void ResetEnemiesSelected()
    {
        enemiesAbilitySelected.Clear();
    }

}