using System;
using System.Collections.Generic;
using State_Machine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using SFXUtil;

public struct PlayerTargetedEvent : IEvent
{

}

public struct PlayerLostTargetEvent : IEvent
{

}

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

struct CorrectShotEvent : IEvent
{
    public EnemyBase enemy;

    public CorrectShotEvent(EnemyBase enemyRefrence) => enemy = enemyRefrence;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : EntityBase, IEntity
{
    [Header("Enemy Configuration")]
    public Animator animator;
    public EnemyConfig_SO enemyData;
    public Vector3 defaultPos;
    public Transform target;
    public float enemyScoreValue;
    [Range(0, 1)] public float petalDropChance;
    
    //[Header("EnemyFields")]
    //Non-Serializable Fields
    protected NavMeshAgent __nmAgent;
    protected StateMachine __enemyStateMachine;
    protected Bounds _enemyAreaBounds;
    
    [NonSerialized]
    public Coroutine attackRoutine = null;

    protected bool _isDead;
    
    //Properties
    public bool IsDead => _isDead;
    
    //Events
    protected EventBindings<RoomPlayerEnterEvent> __playerRoomEnterEventListener;
    protected EventBindings<RoomPlayerExitEvent> __playerRoomExitEventListener;

    [Header("Audio Paths")]
    public EventReference onEnemyAttackEventPath;
    public EventReference enemyPassiveIdle;

    public EventInstance enemyPassiveSFXEvent;

    public EventReference exitIdleAlert;

    protected override void Awake()
    {
        base.Awake();
        defaultPos =  transform.position;
    }
    
    protected override void Start()
    {
        base.Start();
        Initialise();
    }
    
    public void LookAtTarget()
    {
        if (target == null) return;

        var targetPos = target.position;
        targetPos.y = transform.position.y;
        
        transform.LookAt(targetPos);
    }

    protected virtual void Initialise()
    {
        //enemyPassiveSFXEvent = SFXUtilities.CreateEventInstance(enemyPassiveIdle, this.gameObject);

        //Field Init
        __nmAgent = GetComponent<NavMeshAgent>();
        __enemyStateMachine = new StateMachine();
        
        __nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();
        
        var player = GameObject.FindWithTag("Player");
        
        //StateMachine Init
        InitialiseStateMachine();
        
        if (player != null)
        {
            var collider = player.GetComponentInChildren<Collider>();
            
            //print(_enemyAreaBounds);

            if (_enemyAreaBounds.Intersects(collider.bounds))
            {
                target = player.transform;
                FreezeEnemy(false);
            }
            else
            {
                FreezeEnemy(true);
            }
        }
        
        Debug.Log("Enemy Initialised");
    }

    public void Initialise(EnemyConfig_SO config)
    {
        enemyData = config;
        InitialiseWeaknesses();
        Initialise();
    }
    
    protected virtual void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyBase>(this);
        var chaseState = new EnemyChaseState<EnemyBase>(this);
        var attackState = new EnemyAttackState<EnemyBase>(this);
        var deathState = new EnemyDeathState<EnemyBase>(this);
        var spawnState = new EnemySpawnState<EnemyBase>(this);
        
        __enemyStateMachine.AddTransition(idleState, chaseState, new FuncPredicate( ()=> !InDefaultPosRange() || target != null ));
        __enemyStateMachine.AddTransition(chaseState, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        __enemyStateMachine.AddTransition(chaseState, attackState, new FuncPredicate( ()=>InAttackRange() ));
        __enemyStateMachine.AddTransition(attackState, idleState, new FuncPredicate( ()=>!InAttackRange() && attackRoutine == null));
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( ()=>IsDead ) );
        
        if (animator != null)
        {
            __enemyStateMachine.AddAnyTransition(spawnState, new FuncPredicate(()=> animator.GetBool("Spawning")));
            __enemyStateMachine.AddTransition(spawnState, idleState, new FuncPredicate(()=> !animator.GetBool("Spawning")));
        }
        
        __enemyStateMachine.SetState(idleState);

        print("StateMachine Initialised");
    }

    protected virtual void OnEnable()
    {
        base.OnEnable();
        __playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        __playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(__playerRoomExitEventListener);
    }

    protected virtual void OnDisable()
    {
        base.OnDisable();
        EventBus<RoomPlayerEnterEvent>.Unregister(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(__playerRoomExitEventListener);
    }
    
    protected virtual void Update()
    {
        __enemyStateMachine.Update();
    }
    
    public bool InAttackRange()
    {
        if(target == null) return false;
        return Vector3.Distance(target.position, transform.position) < enemyData.attackRange;
    }

    public void FreezeEnemy(bool freeze)
    {
        StopAgent(freeze);
        animator.speed = freeze ? 0 : 1;
    }

    public bool InDefaultPosRange()
    {
        return Vector3.Distance(transform.position, defaultPos) < 1f;
    }

    [SerializeField] private bool _ignoreNavMesh;
    
    public void StopAgent(bool stop)
    {
        if (_ignoreNavMesh) return;
        
        __nmAgent.isStopped = stop;
    }
    
    public void ClearPath()
    {
        __nmAgent.ResetPath();
    }
    
    public void SetTarget(Transform target)
    {
        if (_ignoreNavMesh) return;
        
        if (target == null)
        {
            __nmAgent.destination = defaultPos;
            return;
        }

        __nmAgent.destination = target.position;
    }
    
    private void OnPlayerRoomEnter(RoomPlayerEnterEvent context)
    {
        FreezeEnemy(false);
        
        var playerTransform =  context.playerTransform;

        if (context.room.Bounds != _enemyAreaBounds) return;

        EventBus<PlayerTargetedEvent>.Raise(new PlayerTargetedEvent());
        Debug.Log("Is Entering");
        target = playerTransform;
    }

    private void OnPlayerRoomExit(RoomPlayerExitEvent context)
    {
        Debug.Log("Is Exiting");
        
        var roomBounds = context.room.Bounds;
        
        if(_enemyAreaBounds != roomBounds) return;

        EventBus<PlayerLostTargetEvent>.Raise(new PlayerLostTargetEvent());
        target = null;
    }
    
    public override void OnShot( Weakness weakness, WeakTypes damageType)
    {
        int weaknessCount = Weaknesses.Count;
        
        //print("Weakness before first fail state");
        if (!Weaknesses.Contains(weakness))
            return;
        //("Weakness past first fail state");
        if(weakness.WeakType.HasFlag(damageType))
            weakness.RemoveWeakType(damageType);
        else
        {
            EventBus<WrongShotEvent>.Raise(new WrongShotEvent(this));
            EventBus<WipeComboEvent>.Raise(new WipeComboEvent());
        }


        if(weakness.WeakType == WeakTypes.NONE)
        {
            Weaknesses.Remove(weakness);
            Destroy(weakness.transform.parent.gameObject);
            EventBus<ChangeScoreEvent>.Raise(new ChangeScoreEvent("Hit", 50f));
            EventBus<CorrectShotEvent>.Raise(new CorrectShotEvent(this));
        }

        if (Weaknesses.Count == 0)
        {
            animator.SetTrigger("Death");
            EventBus<PlayerLostTargetEvent>.Raise(new PlayerLostTargetEvent());
            EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent(this));
            EventBus<ChangeScoreEvent>.Raise(new ChangeScoreEvent("Kill", enemyScoreValue));
            _isDead = true;

            var random = Random.value;
            if (random <= petalDropChance) EventBus<PetalSpawnEvent>.Raise(new PetalSpawnEvent(transform.position));
        }
        
        if (!__sequentialWeaknesses) return;
        
        if (Weaknesses.Count < weaknessCount && Weaknesses.Count > 0)
        {
            //print("WEAKNESS SHOT");
            defaultWeaknessTypes.RemoveAt(0);
            Weaknesses[0].ToggleHitbox(true);
            Weaknesses[0].SetWeakType(defaultWeaknessTypes[0]);
        }

        //print("Enemy Base Shot");
    }

    public virtual void StopAllStateRoutines()
    {
        StopAllCoroutines();
    }
}
