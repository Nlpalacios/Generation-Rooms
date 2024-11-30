using UnityEngine;

public class AttackState : IEnemyState
{
    public void EnterState(EnemyBasicStates enemy)
    {
        //Debug.Log("Attack State");
    }

    public void UpdateState(EnemyBasicStates enemy)
    {
        enemy.Attack();
    }

    public void ExitState(EnemyBasicStates enemy)
    {
        //Debug.Log("Exiting attack State");
    }  
}
