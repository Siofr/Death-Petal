using UnityEngine;

public class EnemyDeathState<T>: EnemyBaseState<T> where T : EnemyBase
{
	public EnemyDeathState(T enemyController) : base(enemyController) { }
	
	public override void OnEnter()
    {
	    enemyController.StopAllCoroutines();
	    enemyController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
	    enemyController.animator.SetTrigger("Death");
        enemyController.ClearPath();
    }
}