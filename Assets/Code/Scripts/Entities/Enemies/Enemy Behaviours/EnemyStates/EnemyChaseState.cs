using UnityEngine;

public class EnemyChaseState<T> : EnemyBaseState<T> where T: EnemyBase
{
    public EnemyChaseState(T enemyController) : base(enemyController) { }
    
    public override void Update()
    {
        enemyController.SetTarget(enemyController.target);
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Chase State");
        //enemyController.animator.SetFloat(Animator.StringToHash("Speed"),1f);
        enemyController.StartCoroutine(LerpBlendState("Speed", 1f, 1f));
    }

    public override void OnExit()
    {
        //enemyController.animator.SetFloat(Animator.StringToHash("Speed"),0f);
        enemyController.StartCoroutine(LerpBlendState("Speed", 0f, 1f));
    }
}