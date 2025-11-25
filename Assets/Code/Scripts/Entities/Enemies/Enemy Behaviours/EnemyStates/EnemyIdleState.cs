using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyBase enemyController) : base(enemyController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}