using System;
using System.Collections.Generic;
using State_Machine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
public class EnemyBase : EntityBase, IEntity, ISaveable<EnemySaveData>
{
    [Header("Enemy Configuration")]
    protected EnemySaveData __saveData;
    public Animator animator;
    public EnemyConfig_SO enemyData;
    public Vector3 defaultPos;
    public Transform target;
    [Range(0, 1)] public float petalDropChance;
    
    //[Header("EnemyFields")]
    //Non-Serializable Fields
    private NavMeshAgent _nmAgent;
    protected StateMachine __enemyStateMachine;
    private Bounds _enemyAreaBounds;
    
    [NonSerialized]
    public Coroutine attackRoutine = null;

    private bool _isDead;
    
    //Properties
    public EnemySaveData SaveInfo => __saveData;
    public bool IsDead => _isDead;
    
    //Events
    protected EventBindings<RoomPlayerEnterEvent> __playerRoomEnterEventListener;
    protected EventBindings<RoomPlayerExitEvent> __playerRoomExitEventListener;

    [Header("Audio Paths")]
    public EventReference onEnemyAttackEventPath;
    
    protected override void Awake()
    {
        base.Awake();
        defaultPos =  transform.position;
    }
    
    private void Start()
    {
        Initialise();
    }
    
    public void LookAtTarget()
    {
        if (target == null) return;

        var targetPos = target.position;
        targetPos.y = transform.position.y;
        
        transform.LookAt(targetPos);
    }

    private void Initialise()
    {
        //Field Init
        _nmAgent = GetComponent<NavMeshAgent>();
        __enemyStateMachine = new StateMachine();

        _nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();

        var player = GameObject.FindWithTag("Player");
        
        if (player != null && _enemyAreaBounds.Contains(player.transform.position)) target = player.transform;
        
        //StateMachine Init
        InitialiseStateMachine();
        
        Debug.Log("Enemy Initialised");
    }

    public void Initialise(EnemyConfig_SO config)
    {
        enemyData = config;
        Initialise();
    }
    
    protected virtual void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyBase>(this);
        var chaseState = new EnemyChaseState<EnemyBase>(this);
        var attackState = new EnemyAttackState<EnemyBase>(this);
        var deathState = new EnemyDeathState<EnemyBase>(this);
        
        __enemyStateMachine.AddTransition(idleState, chaseState, new FuncPredicate( ()=> !InDefaultPosRange() || target != null ));
        __enemyStateMachine.AddTransition(chaseState, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        __enemyStateMachine.AddTransition(chaseState, attackState, new FuncPredicate( ()=>InAttackRange() ));
        __enemyStateMachine.AddTransition(attackState, idleState, new FuncPredicate( ()=>!InAttackRange() && attackRoutine == null));
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( ()=>IsDead ) );
        
        __enemyStateMachine.SetState(idleState);

    }

    protected void OnEnable()
    {
        __playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        __playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(__playerRoomExitEventListener);
    }

    protected virtual void OnDisable()
    {
        EventBus<RoomPlayerEnterEvent>.Unregister(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(__playerRoomExitEventListener);
    }
    
    private void Update()
    {
        __enemyStateMachine.Update();
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
            EventBus<CorrectShotEvent>.Raise(new CorrectShotEvent(this));
        }

        if (Weaknesses.Count == 0)
        {
            animator.SetTrigger("Death");
            EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent(this));
            _isDead = true;

            var random = Random.value;
            if (random <= petalDropChance) EventBus<PetalSpawnEvent>.Raise(new PetalSpawnEvent(transform.position));
        }
    }

    public SaveData GetSaveData(LevelData levelData)
    {
        if (__saveData == null)
        {
            var dataInstance = ScriptableObject.CreateInstance<EnemySaveData>();
            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(dataInstance, levelData.AssetSavePath + $"/{gameObject.name}SaveData.asset");
            #endif

            __saveData = dataInstance;
            __saveData.Save(transform.position, Weaknesses);
        }
        
        return __saveData;
    }

    public void LoadSaveData(SaveData levelData)
    {
        __saveData = (EnemySaveData)levelData;
        
        __saveData.Load(transform, Weaknesses);
    }

    public void SaveData()
    {
        __saveData.Save(transform.position, Weaknesses);
    }

    public virtual void StopAllStateRoutines()
    {
        StopAllCoroutines();
    }
}
