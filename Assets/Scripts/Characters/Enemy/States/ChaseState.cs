using UnityEngine;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyBasicStates enemy)
    {
        Debug.Log("Chase State");
    }

    public void UpdateState(EnemyBasicStates enemy)
    {
        Player player = enemy.gameManager.GetPlayer;
        if (!enemy.isCanMove || player == null) return;

        enemy.Chase();
        enemy.NMA_agent.SetDestination(player.transform.position);
        float distancePlayer = Vector3.Distance(enemy.transform.position, player.transform.position);


        if (distancePlayer < enemy.enemyParameters.attackRange)
        {
            enemy.SwitchToState(EnemyStates.Attack);
        }
    }

    public void ExitState(EnemyBasicStates enemy)
    {
        Debug.Log("Exiting Chase State");
        // Limpieza al salir del estado Chase
    }
}
