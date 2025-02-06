using UnityEngine;

public class RockObject : MonoBehaviour, IAbilityPrefab
{
    [SerializeField] private BoxCollider2D boxCollider;
    private Transform targetEnemy;
    private int TotalDamage = 0;

    public void Init(Transform target, int damage)
    {
        targetEnemy = target;
        TotalDamage = damage;

        transform.position = targetEnemy.position;
    }

    //Animation functions
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    public void SwitchCollider()
    {
        boxCollider.enabled = !boxCollider.enabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.TryGetComponent(out HealthControl enemy))
        {
            enemy.RemoveHearts(TotalDamage);
        }
    }
}
