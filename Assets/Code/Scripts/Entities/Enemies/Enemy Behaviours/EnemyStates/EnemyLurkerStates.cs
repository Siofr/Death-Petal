using System.Collections;
using UnityEngine;

public class EnemyLurkerFreezeState : EnemyBaseState<EnemyLurker>
{
    public EnemyLurkerFreezeState(EnemyLurker enemyController) : base(enemyController) { }

    public override void OnEnter()
    {
        Debug.Log("Entering Frozen State");
        enemyController.ToggleFreeze(true);
    }

    public override void OnExit()
    {
        enemyController.ToggleFreeze(false);
    }
}