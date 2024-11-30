using UnityEngine;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyBasicStates enemy)
    {
        //Debug.Log("Chase State");
    }

    public void UpdateState(EnemyBasicStates enemy)
    {
        enemy.Chase();

        enemy.ChaseBasicPlayer(() =>
        {
            if (enemy.canAttack && enemy.haveAttackState) 
                enemy.SwitchToState(EnemyStates.Attack);
        }); 
    }

    public void ExitState(EnemyBasicStates enemy)
    {
        //Debug.Log("Exiting Chase State");
    }
}
