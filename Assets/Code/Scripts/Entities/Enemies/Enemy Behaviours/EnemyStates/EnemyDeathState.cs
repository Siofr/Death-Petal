using UnityEngine;

public class EnemyDeathState: EnemyBaseState
{
	public EnemyDeathState(EnemyBase enemyController) : base(enemyController) { }
	
	public override void OnEnter()
    {
	    enemyController.StopAllCoroutines();
	    enemyController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        enemyController.ClearPath();
    }
}