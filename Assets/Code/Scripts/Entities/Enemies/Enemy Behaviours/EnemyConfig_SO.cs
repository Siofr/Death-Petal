using UnityEngine;

public abstract class EnemyConfig_SO : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed;
    
    [Header("Attack")]
    public float attackSpeed;
    public float attackRange;
    public float attackCooldown;

    [Header("Threat Level (for Dynamic Music)")]
    public int threatLevel;
}

