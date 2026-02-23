using State_Machine;
using Unity.VisualScripting;
using UnityEngine;

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
        base.InitialiseStateMachine();

        var freezeState = new EnemyLurkerFreezeState(this);
        
        __enemyStateMachine.AddAnyTransition(freezeState, new FuncPredicate(()=> _isTargeted));
    }
    
    public void CheckIfTargeted(ActiveTargetEvent context)
    {
        if(context.activeTarget != null)return; 
        
        if(Weaknesses.Contains(context.activeTarget.GetComponent<Weakness>()))
        {
            _isTargeted = false;
            return;
        }

        _isTargeted = true;
    }
    
    public void ToggleFreeze(bool toggle)
    {
        __nmAgent.speed = toggle ? enemyData.attackSpeed : 0f;
        animator.speed = toggle ? 1 : 0f;
    }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);
        
        //TO REMOVE JUST FOR TESTING
        if(Weaknesses.Count < 1) Destroy(gameObject);
    }
}
