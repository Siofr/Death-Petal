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

public class BossBase : EnemyBase
{
    [Header("Boss Settings")]
    [SerializeField] [Tooltip("Percent of Weaknesses remaining to enter Phase 2")]
    private int _phaseTwoThreshold;

    private int _startWeaknessesCount = 0;
    private int _activePhase = 1;

    public Transform debugTarget;
    public bool isAttackReady = true;
    public Bishop_AttackPatternSpawner attackPatternSpawner;
    [SerializeField] private GameObject[] eyes;
    public Bishop_Phase1Attacks activeAttack = Bishop_Phase1Attacks.None;
    
    
    
    
    protected override void Awake()
    {
        target = debugTarget;
        base.Awake();
        defaultPos =  transform.position;
    }
    
    protected override void Start()
    {
        base.Awake();
    }

    protected override void Initialise()
    {
        //Field Init
        //print("Init Boss!");
        __nmAgent = GetComponent<NavMeshAgent>();
        __enemyStateMachine = new StateMachine();
        attackPatternSpawner = GetComponent<Bishop_AttackPatternSpawner>();

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();

        InitialiseStateMachine();
        
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
        var idleState = new BossIdleState<BossBase>(this);
        var attack1State = new BossAttackStage1State<BossBase>(this);
        var attack2State = new BossAttackStage2State<BossBase>(this);
        var defeatState = new BossDefeatState<BossBase>(this);
        
        __enemyStateMachine.AddTransition(idleState, attack1State, new FuncPredicate( ()=> isAttackReady && target != null ));
        __enemyStateMachine.AddTransition(attack1State, idleState, new FuncPredicate( () => target == null || !isAttackReady ));
        
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
        
        foreach (GameObject eye in eyes)
        {
            eye.GetComponent<Bishop_EyeLookAt>().lookAtTarget = playerTransform;
        }
        
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
            Death();

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

    // temporary death thing
    private void Death()
    {
        Destroy(gameObject);
    }
}

