using System;
using System.Collections;
using System.Collections.Generic;
using State_Machine;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileBase : EnemyBase
{
    [Header("Projectile Settings")]
    public Vector3 startVector;
    public int lifeTime;
    public bool active;
    private bool _staticDirection;
    private bool _projectileTarget = false;
    [SerializeField] private List<ParticleSystem> _particleEmitters;
    private EnemyBase _callingEntity;
    [SerializeField] private GameObject explodeVFX;

    private EventBindings<BossKilledEvent> _onBossDeathEvent;
    
    protected override void Awake()
    {
        base.Awake();
        defaultPos =  transform.position;
    }
    
    private void Start()
    {
        Initialise();
    }

    private void DealDamage(EnemyBase callingEntity)
    {
        IEntity playerEntity;
        
        target.TryGetComponent(out playerEntity);

        if (playerEntity.Weaknesses.Count < 1) return;
        
        //print("Deal Damage");
        playerEntity.OnShot(playerEntity.Weaknesses[0], WeakTypes.PLAYER);
        Debug.Log("Damage Dealt to Player");
        
        Debug.Log("Attack Phase Over");
    }

    public void SendProjectile(Vector3 direction, int lifetime, bool isPhysicsObject, EnemyBase callingEntity, Transform targetTransform = null, bool projectileTarget = false)
    {
        //print("I LIVE");
        active = true;
        
        startVector = direction;
        target = callingEntity.target;
        StartCoroutine(KillTimer(lifetime));
        _projectileTarget = projectileTarget;
        _callingEntity = callingEntity;

        if (isPhysicsObject)
        {
            var newRB = gameObject.AddComponent<Rigidbody>();
            newRB.mass = 0.5f;
            
            newRB.AddForce(direction * 4, ForceMode.Impulse);
        }
        else
        {
            _staticDirection = true;
        }

        
    }

    private IEnumerator KillTimer(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (active)
        {
            Explode();
        }
        //explode
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground")) Explode();
    }*/

    private void Explode()
    {
        // do damage in radius
        
        if (Vector3.Distance(target.position, transform.position) < enemyData.attackRange)
        {
            Instantiate(explodeVFX, transform.position, quaternion.identity);
            DealDamage(_callingEntity);
        }
        
        Destroy(gameObject);
    }

    private void Initialise()
    {
        //Field Init
        //print("Init Boss!");
        /*_enemyStateMachine = new StateMachine();

        _nmAgent.speed = enemyData.movementSpeed;

        _enemyAreaBounds = GetComponentInParent<Room>() != null ? GetComponentInParent<Room>().Bounds : new Bounds();
        
        //StateMachine Init
        var idleState = new ProjectileIdleState(this);
        var launchState = new ProjectileLaunchState(this);
        var explodeState = new ProjectileExplodeState(this);
        
        _enemyStateMachine.AddTransition(idleState, launchState, new FuncPredicate( ()=> active));
        _enemyStateMachine.AddTransition(launchState, explodeState, new FuncPredicate( () => target != null && InDamageRange()));
        
        _enemyStateMachine.SetState(idleState);*/
        
        //Event Init
        __playerRoomExitEventListener = new EventBindings<RoomPlayerExitEvent>(OnPlayerRoomExit);
        
        EventBus<RoomPlayerEnterEvent>.Register(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Register(__playerRoomExitEventListener);
        
        //Debug.Log("Enemy Initialised");
    }

    private void OnDisable()
    {
        EventBus<RoomPlayerEnterEvent>.Unregister(__playerRoomEnterEventListener);
        EventBus<RoomPlayerExitEvent>.Unregister(__playerRoomExitEventListener);
        
        EventBus<BossKilledEvent>.Unregister(_onBossDeathEvent);
        
        StopAllCoroutines();
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();

        _onBossDeathEvent = new EventBindings<BossKilledEvent>(OnBossDeath);
        EventBus<BossKilledEvent>.Register(_onBossDeathEvent);
    }

    private void OnBossDeath()
    {
        Destroy(gameObject);
    }
    
    private void Update()
    {
        /*_enemyStateMachine.Update();*/
        if (_staticDirection && !InDamageRange())
        {
            transform.position += startVector;
        }

        if (active && InDamageRange())
        {
            active = false;
            Explode();
            //Explode
        }
    }
    
    public bool InDamageRange()
    {
        if(target == null) return false;
        return Vector3.Distance(target.position, transform.position) < enemyData.attackRange;
    }
    

    private void OnPlayerRoomExit(RoomPlayerExitEvent context)
    {
        Debug.Log("Is Exiting");
        
        var roomBounds = context.room.Bounds;
        Explode();
        
        if(_enemyAreaBounds != roomBounds) return;
        
        target = null;
    }
}

