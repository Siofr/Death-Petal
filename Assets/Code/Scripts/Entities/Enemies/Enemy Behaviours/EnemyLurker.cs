using State_Machine;
using Unity.VisualScripting;
using UnityEngine;
using StateMachine = State_Machine.StateMachine;

public class EnemyLurker : EnemyBase
{
    //Non-Serialized Fields
    private bool _isTargeted;
    
    //Event Fields
    private EventBindings<ActiveTargetEvent> _onTargetedListener;

    protected override void OnEnable()
    {
        base.OnEnable();

        _onTargetedListener = new EventBindings<ActiveTargetEvent>(CheckIfTargeted);
        EventBus<ActiveTargetEvent>.Register(_onTargetedListener);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        EventBus<ActiveTargetEvent>.Unregister(_onTargetedListener);
    }

    protected override void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyLurker>(this);
        var chaseState = new EnemyChaseState<EnemyLurker>(this);
        var attackState = new EnemyAttackState<EnemyLurker>(this);
        var deathState = new EnemyDeathState<EnemyLurker>(this);
        var freezeState = new EnemyLurkerFreezeState(this);
        var spawnState = new EnemySpawnState<EnemyLurker>(this);
        
        __enemyStateMachine.AddTransition(idleState, chaseState, new FuncPredicate( ()=> !InDefaultPosRange() || target != null ));
        __enemyStateMachine.AddTransition(chaseState, idleState, new FuncPredicate( () => target == null && InDefaultPosRange() ));
        
        __enemyStateMachine.AddTransition(chaseState, attackState, new FuncPredicate( ()=>InAttackRange() ));
        __enemyStateMachine.AddTransition(attackState, idleState, new FuncPredicate( ()=>!InAttackRange() && attackRoutine == null));
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( ()=>IsDead ) );
        
        __enemyStateMachine.AddTransition(chaseState, freezeState, new FuncPredicate(()=> _isTargeted));
        __enemyStateMachine.AddTransition(freezeState, chaseState, new FuncPredicate(()=> !_isTargeted));

        if (animator != null)
        {
            __enemyStateMachine.AddTransition(idleState, spawnState, new FuncPredicate(()=> animator.GetBool("Spawning")));
            __enemyStateMachine.AddTransition(spawnState, idleState, new FuncPredicate(()=> !animator.GetBool("Spawning")));
        }
            
        
        __enemyStateMachine.SetState(idleState);
    }

    protected override void Update()
    {
        base.Update();
        //print(__enemyStateMachine.GetActiveState());
    }
    
    public void CheckIfTargeted(ActiveTargetEvent context)
    {
        if (context.activeTarget == null)
        {
            _isTargeted = false;
            return;
        } 
        
        if(!Weaknesses.Contains(context.activeTarget.GetComponent<Weakness>()))
        {
            _isTargeted = false;
            return;
        }

        _isTargeted = true;
    }
    
    public void ToggleFreeze(bool toggle)
    {
        StopAgent(toggle);
        animator.speed = toggle ? 0f : 1f;
    }
    
    // public override void OnShot(Weakness weakness, WeakTypes damageType)
    // {
    //     base.OnShot(weakness, damageType);
    //     
    //     //TO REMOVE JUST FOR TESTING
    //     if(Weaknesses.Count < 1) Destroy(gameObject);
    // }
}
