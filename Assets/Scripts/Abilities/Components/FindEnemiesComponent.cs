using System;
using UnityEngine;

public class FindEnemiesComponent : Component
{
    public void IntantiateAbilityPrefab(GameObject prefab, int countEnemies, int damage, Action<GameObject> enemyAction = null)
    {
        for (int i = 0; i < countEnemies; i++)
        {
            GameObject enemy = EnemyManager.Instance.GetDiferentActiveEnemy();
            if (enemy == null)
            {
                continue;
            }

            IAbilityPrefab NewPrefab = Instantiate(prefab).GetComponent<IAbilityPrefab>();
            if (NewPrefab == null)
            {
                Debug.LogWarning("No Interface available for ability instantiation.");
                continue;
            }

            NewPrefab.Init(enemy.transform, damage);
            enemyAction?.Invoke(enemy);
        }

        EnemyManager.Instance.ResetEnemiesSelected();
    }

    public void InstantiateAbilityAndFreeze(GameObject prefab, int countEnemies, int damage, float seconds)
    {
        IntantiateAbilityPrefab(prefab, countEnemies, damage, (GameObject enemyObject) => 
        {
            if (enemyObject.TryGetComponent<EnemyBasicStates>(out EnemyBasicStates enemyBasicStates))
            {
                enemyBasicStates.FrezeeEnemy(seconds);
            }
            else
            {
                Debug.LogError("Enemy no contains a EnemyBasic Component");
            }
        });
    }

}
