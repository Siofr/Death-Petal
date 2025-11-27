using System;
using System.Collections.Generic;
using State_Machine;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

struct EnemyDeathEvent: IEvent
{
    public EnemyBase enemy;
    
    public EnemyDeathEvent(EnemyBase enemyReference) => enemy = enemyReference;
}

struct WrongShotEvent : IEvent
{
    public EnemyBase enemy;

    public WrongShotEvent(EnemyBase enemyRefrence) => enemy = enemyRefrence;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : EntityBase
{
    [Header("Enemy Configuration")]
    public Animator animator;
    public EnemyConfig_SO enemyData;
    public Vector3 defaultPos;
    public Transform target;
    
    //[Header("EnemyFields")]
    //Non-Serializable Fields
    private NavMeshAgent _nmAgent;
    private StateMachine _enemyStateMachine;
    private Bounds _enemyAreaBounds;
    
    [NonSerialized]
    public Coroutine attackRoutine = null;

    private bool _isDead;
    
    //Properties
    public bool IsDead => _isDead;
    
    //Events
    private EventBindings<RoomPlayerEnterEvent> _playerRoomEnterEventListener;
    private EventBindings<RoomPlayerExitEvent> _playerRoomExitEventListener;

    protected override void Awake()
    {
        base.Awake();
        defaultPos =  transform.position;
    }
    
    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        //Field Init
        _nmAgent = GetComponent<NavMeshAgent>();
        _enemyStateMachine = new StateMachine();

        _nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();
        
        //StateMachine Init
        var idleState = new EnemyIdleState(this);
        var chaseState = new EnemyChaseState(this);
        var attackState = new EnemyAttackState(this);
        var deathState = new EnemyDeathState(this);
        
        _enemyStateMachine.AddTransition(idleState, chaseState, new FuncPredicate( ()=> !InDefaultPosRange() || target != null ));
        _enemyStateMachine.AddTransition(chaseState, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        _enemyStateMachine.AddTransition(chaseState, attackState, new FuncPredicate( ()=>InAttackRange() ));
        _enemyStateMachine.AddTransition(attackState, idleState, new FuncPredicate( ()=>!InAttackRange() && attackRoutine == null));
        
        _enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( ()=>IsDead ) );
        
        _enemyStateMachine.SetState(idleState);
        
        //Event Init
        _playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        _playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(_playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(_playerRoomExitEventListener);
        
        Debug.Log("Enemy Initialised");
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
    
    public bool InAttackRange()
    {
        if(target == null) return false;
        return Vector3.Distance(target.position, transform.position) < enemyData.attackRange;
    }

    public bool InDefaultPosRange()
    {
        return Vector3.Distance(transform.position, defaultPos) < 1f;
    }
    
    public void ClearPath()
    {
        _nmAgent.ResetPath();
    }
    
    public void SetTarget(Transform target)
    {
        
        if (target == null)
        {
            _nmAgent.destination = defaultPos;
            return;
        }
        
        _nmAgent.destination = target.position;
    }
    
    private void OnPlayerRoomEnter(RoomPlayerEnterEvent context)
    {
        var playerTransform =  context.playerTransform;

        if (context.room.Bounds != _enemyAreaBounds) return;
        
        Debug.Log("Is Entering");
        target = playerTransform;
    }

    private void OnPlayerRoomExit(RoomPlayerExitEvent context)
    {
        Debug.Log("Is Exiting");
        
        var roomBounds = context.room.Bounds;
        
        if(_enemyAreaBounds != roomBounds) return;
        
        target = null;
    }
    
    public override void OnShot( Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness))
            return;
        
        if(weakness.WeakType.HasFlag(damageType))
            weakness.RemoveWeakType(damageType);
        else
            EventBus<WrongShotEvent>.Raise(new WrongShotEvent(this));

        if(weakness.WeakType == WeakTypes.NONE)
        {
            Weaknesses.Remove(weakness);
            Destroy(weakness.transform.parent.gameObject);
        }

        if (Weaknesses.Count == 0)
        {
            animator.SetTrigger("Death");
            EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent(this));
            _isDead = true;
        }
    }
}
