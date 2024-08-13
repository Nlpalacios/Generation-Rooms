using UnityEngine;

public interface IEnemyState 
{
    void EnterState(EnemyBasicStates enemy);
    void UpdateState(EnemyBasicStates enemy);
    void ExitState(EnemyBasicStates enemy);
}

public enum EnemyStates
{
    Idle,
    Chase,
    Attack
}