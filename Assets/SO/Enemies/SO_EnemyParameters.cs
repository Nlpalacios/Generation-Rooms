using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyParameters", menuName = "Scriptable Objects/SO_EnemyParameters")]
public class SO_EnemyParameters : ScriptableObject
{
    [Header("Type")]
    public typeEnemy enemyType;

    [Header("Attack")]
    public int damage;
    public float timeDelay;

    [Header("Healt")]
    public int maxHearts;

    [Header("Movement")]
    public float speed;
    public float attackRange;
}

public enum typeEnemy: short
{
    Slime     = 1,
    Wasp      = 2,
    Knight    = 3
}