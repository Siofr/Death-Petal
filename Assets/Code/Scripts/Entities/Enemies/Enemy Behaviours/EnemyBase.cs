using System;
using System.Collections.Generic;
using State_Machine;
using UnityEngine;
using UnityEngine.AI;

struct EnemyDeathEvent: IEvent
{
    public EnemyBase enemy;
    
    public EnemyDeathEvent(EnemyBase enemyReference) => enemy = enemyReference;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour, IEntity
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyConfigData_SO _enemyData;
    public EnemyMovementStrategy_SO enemyMovementStrategy;
    public EnemyAttackStrategy_SO enemyAttackStrategy;
    
    [Header("EnemyFields")]
    [SerializeReference] private Bounds _enemyAreaBounds;
    [SerializeField] private List<Weakness> _weaknesses = new List<Weakness>();

    //Non-Serializable Fields
    private NavMeshAgent _nmAgent;
    private StateMachine _enemyStateMachine;
    
    //Properties
    public List<Weakness> Weaknesses
    {
        get =>  _weaknesses;
    }
    
    //Events
    private EventBindings<EnemyDeathEvent> _enemyDeathEventListener;
    
    private void Awake()
    {
        Initialise();
    }
    
    private void Update()
    {
        _enemyStateMachine.Update();
    }

    private void Initialise()
    {
        _nmAgent = GetComponent<NavMeshAgent>();
        _enemyStateMachine = new();
        
        //StateMachine Init
        var idleState = new EnemyIdleState(this);
        var attackState = new EnemyAttackState(this);
        
        //Event Init
        _enemyDeathEventListener = new EventBindings<EnemyDeathEvent>(OnDeath);
    }

    public void HandleMovement()
    {
        enemyMovementStrategy.Movement();
    }
    
    private void OnDeath(EnemyDeathEvent context)
    {
        
    }
    
    public void OnDamage(Weakness damageType)
    {
        if (!Weaknesses.Contains(damageType))
            return;
        
        Weaknesses.Remove(damageType);
        
        if(Weaknesses.Count == 0)
            EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent(this));
    }
}
