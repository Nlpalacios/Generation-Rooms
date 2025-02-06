using UnityEngine;

public class RayObject : MonoBehaviour, IAbilityPrefab
{
    [SerializeField] private float upDistanceAnimation = 1.5f;
    private Transform targetEnemy;
    private int TotalDamage = 0;

    public void Init(Transform target, int damage)
    {
        targetEnemy = target;
        TotalDamage = damage;
    }

    private void LateUpdate()
    {
        if (!targetEnemy) return; 
        Vector3 finalPos = new Vector3(targetEnemy.position.x, (targetEnemy.position.y + upDistanceAnimation), 1);
        transform.position = finalPos;
    }

    //Animation functions
    public void AttackEnemy()
    {
        if (targetEnemy == null) return;
        if (targetEnemy.gameObject.TryGetComponent(out HealthControl enemy))
        {
            enemy.RemoveHearts(TotalDamage);
        }
        else
        {
            Debug.Log($"Not find a healtControl in Enemy: {targetEnemy.gameObject.name}");
        }
    }

    public void DestroyRay()
    {
        Destroy(this.gameObject);
    }
}
