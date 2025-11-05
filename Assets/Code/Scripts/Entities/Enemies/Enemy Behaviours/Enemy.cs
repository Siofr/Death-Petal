using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyConfigData_SO _enemyData;
    [SerializeField] private EnemyMovementStrategy_SO _enemyMovementStrategy;
    [SerializeField] private EnemyAttackStrategy_SO _enemyAttackStrategy;
}
