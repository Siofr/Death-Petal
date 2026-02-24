using System;
using System.Collections.Generic;
using State_Machine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using Random = UnityEngine.Random;

public class BossBase : EnemyBase, ISaveable<EnemySaveData>
{
    [Header("Boss Settings")]
    [SerializeField] [Tooltip("Percent of Weaknesses remaining to enter Phase 2")]
    private int _phaseTwoThreshold;

    private int _startWeaknessesCount = 0;
    private int _activePhase = 1;
    
    
    
    
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
        print("Init Boss!");
        _nmAgent = GetComponent<NavMeshAgent>();
        __enemyStateMachine = new StateMachine();

        _nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();
        
        
        
        //Event Init
        __playerRoomEnterEventListener = new EventBindings<RoomPlayerEnterEvent>(OnPlayerRoomEnter);
        __playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(__playerRoomExitEventListener);

        _startWeaknessesCount = Weaknesses.Count;
        
        Debug.Log("Enemy Initialised");
    }

    private void OnDisable()
    {
        EventBus<RoomPlayerEnterEvent>.Unregister(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(__playerRoomExitEventListener);
    }
    
    protected virtual void InitialiseStateMachine()
    {
        //StateMachine Init
        var idleState = new BossIdleState(this);
        var attack1State = new BossAttackStage1State(this);
        var attack2State = new BossAttackStage2State(this);
        var defeatState = new BossDefeatState(this);
        
        __enemyStateMachine.AddTransition(idleState, attack1State, new FuncPredicate( ()=> !InDefaultPosRange() || target != null ));
        __enemyStateMachine.AddTransition(attack1State, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        __enemyStateMachine.AddTransition(attack1State, attack2State, new FuncPredicate( () => _activePhase == 2));
        __enemyStateMachine.AddTransition(attack2State, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        __enemyStateMachine.AddAnyTransition(defeatState, new FuncPredicate( ()=>IsDead ) );
        
        __enemyStateMachine.SetState(idleState);

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

        if (Weaknesses.Count / _startWeaknessesCount * 100 <= _phaseTwoThreshold)
        {
            // Start phase two
            _activePhase = 2;
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
}

