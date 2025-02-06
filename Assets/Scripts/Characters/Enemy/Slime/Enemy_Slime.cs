using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Slime : EnemyBasicStates
{
    [Header("Time stopping")]
    [SerializeField] private float timeAnimStopping;
    [SerializeField] private float scaleReductionFactor = 0.2f;
    [SerializeField] private float minScale = 0.8f;
    private bool isStopped = false;

    [Header("Custom properties")]
    [SerializeField] private float NewScale = 1f;
     
    private void Start()
    {
        base.haveAttackState = false;
        isStopped = false;
        NMA_agent.isStopped = false;
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
        float newScale = Mathf.Max(currentScale - scaleReductionFactor * damage, minScale);

        if (newScale <= minScale)
        {
            base.OnDeath();
            return;
        }

        int newHealth = currentHealt.GetCurrentHealth - damage;

        Vector3 posClone = transform.position;
        GameObject newSlimeObject = EnemyManager.Instance.InstantiateEnemy(posClone, typeEnemy.Slime);

        if (newSlimeObject != null)
        {
            transform.localScale = new Vector3(newScale, newScale, newScale);

            Enemy_Slime newSlime = newSlimeObject.GetComponent<Enemy_Slime>();
            if (newSlime != null)
            {
                newSlime.ChangeScaleAndHealth(newScale, newHealth);
            }
        }
    }

    public void ChangeScaleAndHealth(float scale, int health)
    {
        if (scale <= minScale)
        {
            OnDeath();
            return;
        }

        currentHealt.SetMaxHeart = health;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && !isStopped && collision.gameObject.TryGetComponent(out Player player))
        {
            base.AttackPlayer();
        }
    }

    public override void ResetEnemy()
    {
        transform.localScale = new Vector3(NewScale, NewScale, NewScale);
        isStopped = false;
    }

    public override void Chase() { }
    public override void Attack() { }
    public override void StartAttack() { }

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