using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyBase enemyController) : base(enemyController) { }
    
    public override void Update()
    {
        enemyController.SetTarget(enemyController.target);
    }
}