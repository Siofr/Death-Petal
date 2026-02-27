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
    
    [Header("Enemy Sequential Fields")]
    [SerializeField] protected bool _sequentialWeaknesses;
    public List<WeakTypes> defaultWeaknessTypes;
    
    //[Header("EnemyFields")]
    //Non-Serializable Fields
    protected NavMeshAgent __nmAgent;
    protected StateMachine __enemyStateMachine;
    protected Bounds _enemyAreaBounds;
    
    [NonSerialized]
    public Coroutine attackRoutine = null;

    protected bool _isDead;
    
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
        __nmAgent = GetComponent<NavMeshAgent>();
        __enemyStateMachine = new StateMachine();

        __nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();

        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            var collider = player.GetComponentInChildren<Collider>();
            
            //print(_enemyAreaBounds);
            
            if (_enemyAreaBounds.Intersects(collider.bounds)) target = player.transform;
        }
        
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

    protected virtual void OnEnable()
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

    public void StopAgent(bool stop)
    {
        __nmAgent.isStopped = stop;
    }
    
    public void ClearPath()
    {
        __nmAgent.ResetPath();
    }
    
    public void SetTarget(Transform target)
    {
        
        if (target == null)
        {
            __nmAgent.destination = defaultPos;
            return;
        }
        
        __nmAgent.destination = target.position;
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
        int weaknessCount = Weaknesses.Count;
        
        print("Weakness before first fail state");
        if (!Weaknesses.Contains(weakness))
            return;
        print("Weakness past first fail state");
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
        
        if (!__sequentialWeaknesses) return;
        
        if (Weaknesses.Count < weaknessCount && Weaknesses.Count > 0)
        {
            print("WEAKNESS SHOT");
            defaultWeaknessTypes.RemoveAt(0);
            Weaknesses[0].ToggleHitbox(true);
            Weaknesses[0].SetWeakType(defaultWeaknessTypes[0]);
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
