using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyBase enemyController) : base(enemyController) { }
    
    private Coroutine _damageRoutine = null;

    private IEnumerator DealDamage(float attackSpeed)
    {
        IEntity playerEntity;
        
        enemyController.target.TryGetComponent(out playerEntity);
        
        while (_damageRoutine != null && playerEntity.Weaknesses.Count > 0)
        {
            playerEntity.OnShot(playerEntity.Weaknesses[0]);
            yield return new WaitForSeconds(attackSpeed);
        }
    }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        enemyController.SetTarget(null);
    }

    public override void OnExit()
    {
        _damageRoutine = null;
    }
}