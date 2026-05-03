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
    public int threatLevel;

    public PlayerTargetedEvent(int threatLevel)
    {
        this.threatLevel = threatLevel;
    }
}

public struct PlayerLostTargetEvent : IEvent
{
    public int threatLevel;

    public PlayerLostTargetEvent(int threatLevel)
    {
        this.threatLevel = threatLevel;
    }
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

public struct PauseEvent : IEvent
{
    public bool isPaused;

    public static List<string> savedInputs = new List<string>();
    
    public PauseEvent(bool isPaused)
    {
        this.isPaused = isPaused;

        if (isPaused)
        {
            foreach (var input in InputHandler.inputDict)
            {
                if(input.Value.enabled) savedInputs.Add(input.Key);
            }
            
            EntityHelper.LockAllInputs();
        }
        else
        {
            foreach (var input in savedInputs)
            {
                EventBus<UnlockInput>.Raise(new UnlockInput(input));
            }
        }
    }
}

public struct CameraActionEvent : IEvent
{
    public bool isActive;
    public CameraActionEvent(bool isActive) => this.isActive = isActive;
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
    protected bool __isPaused;
    
    //Properties
    public bool IsDead => _isDead;
    
    //Events
    protected EventBindings<RoomPlayerEnterEvent> __playerRoomEnterEventListener;
    protected EventBindings<RoomPlayerExitEvent> __playerRoomExitEventListener;
    protected EventBindings<PauseEvent> __pauseEventListener;
    
    public TriggerSFXState sfxStateManager;

    protected override void Awake()
    {
        base.Awake();
        defaultPos =  transform.position;
    }
    
    protected override void Start()
    {
        base.Start();
        Initialise();
        TryGetComponent<TriggerSFXState>(out sfxStateManager);
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
            __enemyStateMachine.AddTransition(idleState, spawnState, new FuncPredicate(()=> animator.GetBool("Spawning")));
            __enemyStateMachine.AddTransition(spawnState, idleState, new FuncPredicate(()=> !animator.GetBool("Spawning")));
        }
        
        __enemyStateMachine.SetState(idleState);

        print("StateMachine Initialised");
    }

    private void OnPause(PauseEvent ctx)
    {
        __isPaused = ctx.isPaused;
        StopAgent(ctx.isPaused);
        
        __enemyStateMachine.PauseStateMachine(ctx.isPaused);
        
        animator.speed = ctx.isPaused ? 0 : 1;

        if (!ctx.isPaused)
        {
            if (target == null && animator.GetBool(Animator.StringToHash("Spawning")))
            {
                animator.speed = 0;
            }
        }
    }

    protected override void OnCameraChange(CameraChangeEvent ctx)
    {
        base.OnCameraChange(ctx);
        if (animator && animator.GetBool(Animator.StringToHash("Spawning"))) Weaknesses[0].Toggle(false);
        if(IsDead) ToggleAllWeaknesses(false);
    }

    protected virtual void OnEnable()
    {
        base.OnEnable();
        __playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        __playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        __pauseEventListener = new EventBindings<PauseEvent>(OnPause);
        
        EventBus<RoomPlayerEnterEvent>.Register(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(__playerRoomExitEventListener);
        EventBus<PauseEvent>.Register(__pauseEventListener);
    }

    protected virtual void OnDisable()
    {
        base.OnDisable();
        EventBus<RoomPlayerEnterEvent>.Unregister(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(__playerRoomExitEventListener);
        EventBus<PauseEvent>.Unregister(__pauseEventListener);
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
        var playerTransform =  context.playerTransform;

        if (context.room.Bounds != _enemyAreaBounds) return;
        FreezeEnemy(false);
        
        EventBus<PlayerTargetedEvent>.Raise(new PlayerTargetedEvent(enemyData.threatLevel));
        Debug.Log("Is Entering");
        target = playerTransform;
    }

    private void OnPlayerRoomExit(RoomPlayerExitEvent context)
    {
        Debug.Log("Is Exiting");
        
        var roomBounds = context.room.Bounds;
        
        if(_enemyAreaBounds != roomBounds) return;

        EventBus<PlayerLostTargetEvent>.Raise(new PlayerLostTargetEvent(enemyData.threatLevel));
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
            EventBus<PlayerLostTargetEvent>.Raise(new PlayerLostTargetEvent(enemyData.threatLevel));
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

    public override void HandleLoadData(ref LevelSaveData refData)
    {
        base.HandleLoadData(ref refData);
        if (SaveInfo.health.Count < 1)
        {
            gameObject.SetActive(false);
            _isDead = true;
            
            ToggleAllWeaknesses(false);
        }
    }

    public virtual void StopAllStateRoutines()
    {
        StopAllCoroutines();
    }
}
