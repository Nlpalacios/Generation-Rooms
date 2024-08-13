using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnemyParameters", menuName = "Scriptable Objects/SO_EnemyParameters")]
public class SO_EnemyParameters : ScriptableObject
{
    [Header("Type")]
    public typeEnemy enemyType;

    [Header("Damage")]
    public int damage;

    [Header("Healt")]
    public int maxHearts;

    [Header("Movement")]
    public float speed;
    public float attackRange;
}

public enum typeEnemy
{
    Slime,
    Knight,
    Wasp
}