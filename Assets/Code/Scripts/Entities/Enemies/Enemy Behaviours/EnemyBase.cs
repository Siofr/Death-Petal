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
    public EnemyConfig_SO enemyData;
    
    //[Header("EnemyFields")]
    //Non-Serializable Fields
    private NavMeshAgent _nmAgent;
    private StateMachine _enemyStateMachine;
    private Bounds _enemyAreaBounds;
    private List<Weakness> _weaknesses = new List<Weakness>();
    
    public Transform target;
    
    //Properties
    public List<Weakness> Weaknesses => _weaknesses;

    //Events
    private EventBindings<RoomPlayerEnterEvent> _playerRoomEnterEventListener;
    private EventBindings<RoomPlayerExitEvent> _playerRoomExitEventListener;
    
    private void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        //Field Init
        _nmAgent = GetComponent<NavMeshAgent>();
        _enemyStateMachine = new StateMachine();

        _nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>().Bounds;
        
        //StateMachine Init
        var idleState = new EnemyIdleState(this);
        var chaseState = new EnemyChaseState(this);
        var attackState = new EnemyAttackState(this);
        
        _enemyStateMachine.AddTransition(idleState, chaseState, new FuncPredicate( ()=> target!= null ));
        _enemyStateMachine.AddTransition(chaseState, idleState, new FuncPredicate( () => target == null ));
        
        _enemyStateMachine.AddTransition(chaseState, attackState, new FuncPredicate( ()=>InAttackRange() ));
        _enemyStateMachine.AddTransition(attackState, idleState, new FuncPredicate( ()=>!InAttackRange() ));
        
        _enemyStateMachine.SetState(idleState);
        
        //Event Init
        _playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        _playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(_playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(_playerRoomExitEventListener);
    }

    private void OnDisable()
    {
        EventBus<RoomPlayerEnterEvent>.Unregister(_playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(_playerRoomExitEventListener);
    }
    
    private void Update()
    {
        _enemyStateMachine.Update();
    }
    
    private bool InAttackRange()
    {
        if(target == null) return false;
        return Vector3.Distance(target.position, transform.position) < enemyData.attackRange;
    }

    public void SetTarget(Transform target)
    {
        
        if (target == null)
        {
            _nmAgent.ResetPath();
            return;
        }
        
        _nmAgent.destination = target.position;
    }
    
    private void OnPlayerRoomEnter(RoomPlayerEnterEvent context)
    {
        Debug.Log("Is Entering");
        
        var roomBounds = context.room.Bounds;
        
        if (_enemyAreaBounds != roomBounds) return;
        
        var playerTransform =  context.playerTransform;
        target = playerTransform;
    }

    private void OnPlayerRoomExit(RoomPlayerExitEvent context)
    {
        Debug.Log("Is Exiting");
        
        var roomBounds = context.room.Bounds;
        
        if(_enemyAreaBounds != roomBounds) return;
        
        target = null;
    }
    
    public void OnShot(Weakness damageType)
    {
        if (!Weaknesses.Contains(damageType))
            return;
        
        Weaknesses.Remove(damageType);
        
        if(Weaknesses.Count == 0)
            EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent(this));
    }
}
