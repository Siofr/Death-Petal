using UnityEngine;

public class EnemyDeathState<T>: EnemyBaseState<T> where T : EnemyBase
{
	public EnemyDeathState(T enemyController) : base(enemyController) { }
	
	public override void OnEnter()
    {
		enemyController.sfxStateManager?.OnStateEnter("Death");
	    enemyController.StopAllCoroutines();
	    enemyController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
	    enemyController.animator.SetTrigger("Death");
        enemyController.ClearPath();
    }

    public override void OnExit()
    {
        enemyController.sfxStateManager.OnStateExit("Death");
    }
}