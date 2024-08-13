using UnityEngine;

public class AttackState : IEnemyState
{
    public void EnterState(EnemyBasicStates enemy)
    {
        Debug.Log("Attack State");
    }

    public void UpdateState(EnemyBasicStates enemy)
    {
        if (enemy.NMA_agent == null && enemy.gameManager == null) return;

        enemy.Attack();
        enemy.NMA_agent.SetDestination(enemy.gameManager.GetPlayer.transform.position);
        float distancePlayer = Vector3.Distance(enemy.transform.position, enemy.gameManager.GetPlayer.transform.position);

        if (distancePlayer > enemy.enemyParameters.attackRange)
        {
            enemy.SwitchToState(EnemyStates.Chase);
        }
    }

    public void ExitState(EnemyBasicStates enemy)
    {
        Debug.Log("Exiting attack State");
    }  
}
