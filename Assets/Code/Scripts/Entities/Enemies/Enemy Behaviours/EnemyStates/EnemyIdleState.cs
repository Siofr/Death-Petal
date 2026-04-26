using UnityEngine;
using FMODUnity;

public class EnemyIdleState<T> : EnemyBaseState<T> where T: EnemyBase
{
    public EnemyIdleState(T enemyController) : base(enemyController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");

        enemyController.animator.SetTrigger("Spawn");
        enemyController.StopAllStateRoutines();
    }
    
    public override void OnExit()
    {

    }
    
    
}