using System.Collections;
using UnityEngine;
using FMODUnity;

public class EnemyAttackState<T> : EnemyBaseState<T> where T : EnemyBase
{
    public EnemyAttackState(T enemyController) : base(enemyController) { }
    
    private IEnumerator DealDamage(float attackSpeed)
    {
        IEntity playerEntity;
        
        enemyController.target.TryGetComponent(out playerEntity);
        
        while (playerEntity != null && playerEntity.Weaknesses.Count > 0)
        {
            yield return new WaitForSeconds(attackSpeed);
            
            if (!enemyController.InAttackRange()) break;
            
            playerEntity.OnShot(playerEntity.Weaknesses[0], WeakTypes.PLAYER);
            Debug.Log("Damage Dealt to Player");
            
            yield return new WaitForSeconds(enemyController.enemyData.attackCooldown);
        }
        
        yield return new WaitForSeconds(enemyController.enemyData.attackCooldown);
        
        Debug.Log("Attack Phase Over");
        
        enemyController.attackRoutine = null;
    }
    
    public override void OnEnter()
    {
        enemyController.StartCoroutine(LerpBlendState("Speed", 0f, 1f));
        
        enemyController.StopAgent(true);
        
        Debug.Log("Entering Attack State");
        //enemyController.SetTarget(null);
        enemyController.StopAllCoroutines();
        enemyController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        
        enemyController.animator.SetBool(Animator.StringToHash("Attack"), true);
        enemyController.animator.SetTrigger(Animator.StringToHash("Lunge"));
        enemyController.attackRoutine = enemyController.StartCoroutine(DealDamage(enemyController.enemyData.attackSpeed));
    }

    public override void OnExit()
    {
        enemyController.StopAgent(false);
        enemyController.animator.SetBool(Animator.StringToHash("Attack"), false);
        Debug.Log("Exiting Attack State");
    }
}