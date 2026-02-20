using System.Collections;
using System.Collections.Generic;
using State_Machine;
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
        
        callingEntity.target.TryGetComponent(out playerEntity);
        
        playerEntity.OnShot(playerEntity.Weaknesses[0], WeakTypes.PLAYER);
        Debug.Log("Damage Dealt to Player");
        
        Debug.Log("Attack Phase Over");
    }

    public void SendProjectile(Vector3 direction, int lifetime, bool isPhysicsObject, EnemyBase callingEntity, Transform targetTransform = null, bool projectileTarget = false)
    {
        startVector = direction;
        StartCoroutine(KillTimer(lifetime));
        _projectileTarget = projectileTarget;
        _callingEntity = callingEntity;
        
        IEntity playerEntity;
        
        callingEntity.target.TryGetComponent(out playerEntity);

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

        active = true;
    }

    private IEnumerator KillTimer(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (active)
        {
            StartCoroutine(Explode());
        }
        //explode
    }

    private IEnumerator Explode()
    {
        // do damage in radius
        _particleEmitters.ForEach(x => x.Play());
        DealDamage(_callingEntity);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void Initialise()
    {
        //Field Init
        print("Init Boss!");
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
        /*_enemyStateMachine.Update();*/
        if (_staticDirection && !InDamageRange())
        {
            transform.position += startVector;
        }

        if (active && InDamageRange())
        {
            active = false;
            StartCoroutine(Explode());
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
        StartCoroutine(Explode());
        
        if(_enemyAreaBounds != roomBounds) return;
        
        target = null;
    }
    
    
}

