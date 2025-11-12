using UnityEngine;

public abstract class EnemyConfig_SO : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed;
    public float reloadMovementSpeed;
    
    [Header("Attack")]
    public float attackSpeed;
    public float attackRange;
}

