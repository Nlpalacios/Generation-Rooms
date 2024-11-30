using System.Collections;
using UnityEngine;

public class Enemy_Slime : EnemyBasicStates
{
    [Header("Time stopping")]
    [SerializeField] private float timeAnimStopping;
    [SerializeField] private float minScale = 0.8f;
    private bool isStopped = false;

    private void Start()
    {
        base.haveAttackState = false;
    }

    public IEnumerator StopAnimation()
    {
        if (NMA_agent == null || !NMA_agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not active or not on NavMesh.");
            yield break;
        }

        NMA_agent.isStopped = true;
        enemyAnim.enabled = false;
        isStopped = true;

        yield return new WaitForSeconds(timeAnimStopping);

        NMA_agent.isStopped = false;
        enemyAnim.enabled = true;
        isStopped = false;
    }

    public override void OnHealthChanged(int damage)
    {
        float currentScale = transform.localScale.x;
        if (currentScale <= minScale)
        {
            this.OnDeath();
            return;
        }

        currentScale = Mathf.Max(currentScale -= 0.2f, minScale);
        int newHearts = this.currentHealt.GetCurrentHealth - damage;
        
        Vector3 posClone = this.transform.localPosition;
        GameObject enemy = EnemyManager.Instance.InstantiateEnemy(posClone, typeEnemy.Slime);

        if (enemy != null)
        {
            this.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            enemy.GetComponent<Enemy_Slime>().ChangeScaleAndHealth(currentScale, newHearts);
        }
    }

    public void ChangeScaleAndHealth(float scale, int health)
    {
        if (scale <= minScale)
        {
            OnDeath();
            return;
        }
        this.currentHealt.SetMaxHeart = health;
        this.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && !isStopped && collision.gameObject.TryGetComponent(out Player player))
        {
            base.AttackPlayer();
        }
    }

    public override void Chase() {}
    public override void Attack() {}
    public override void StartAttack() {}
}

//PROXIMAMENTE
//IDEA: SI HAY UN MINIMO DE SLIMES Y SON PEQUEÑOS, ESCOGER A UNO PARA QUE TODOS SE UNAN A EL Y SE HAGAN EL DOBLE DE GRANDE, ASI MISMO SU ATAQUE
//private void OnCollisionEnter2D(Collision2D collision)
//{
//    if (!isPowerUp) return;

//    Enemy_Slime slime = collision.gameObject.GetComponent<Enemy_Slime>();

//    if (slime)
//    {
//       powerUp();
//    }
//}