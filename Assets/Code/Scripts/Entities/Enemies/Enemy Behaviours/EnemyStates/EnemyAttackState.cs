using System.Collections;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyBase enemyController) : base(enemyController) { }

    private IEnumerator DealDamage(float attackSpeed)
    {
        IEntity playerEntity;
        
        enemyController.target.TryGetComponent(out playerEntity);
        
        while (playerEntity.Weaknesses.Count > 0 && enemyController.target != null)
        {
            playerEntity.OnShot(playerEntity.Weaknesses[0]);
            Debug.Log("Damage Dealt to Player");
            
            yield return new WaitForSeconds(attackSpeed);
        }
        
        Debug.Log("Attack Phase Over");
    }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        enemyController.SetTarget(null);

        enemyController.StartCoroutine(DealDamage(enemyController.enemyData.attackSpeed));
    }

    public override void OnExit()
    {
        enemyController.StopAllCoroutines();
        Debug.Log("Exiting Attack State");
    }
}