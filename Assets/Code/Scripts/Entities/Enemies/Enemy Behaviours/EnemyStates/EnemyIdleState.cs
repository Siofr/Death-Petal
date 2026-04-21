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
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(enemyController.enemyPassiveSFXEvent, enemyController.gameObject));
    }
    
    public override void OnExit()
    {
        enemyController.enemyPassiveSFXEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.PlayOneShot(enemyController.exitIdleAlert, enemyController.transform.position);
    }
    
    
}