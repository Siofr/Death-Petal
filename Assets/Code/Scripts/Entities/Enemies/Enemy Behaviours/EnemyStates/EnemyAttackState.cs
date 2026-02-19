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
        
        yield return new WaitForSeconds(attackSpeed);
        
        while (playerEntity.Weaknesses.Count > 0)
        {
            playerEntity.OnShot(playerEntity.Weaknesses[0], WeakTypes.PLAYER);
            Debug.Log("Damage Dealt to Player");
            
            yield return new WaitForSeconds(attackSpeed);

            if (!enemyController.InAttackRange()) break;
        }
        
        Debug.Log("Attack Phase Over");
        
        enemyController.attackRoutine = null;
    }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        //enemyController.SetTarget(null);

        RuntimeManager.PlayOneShot(enemyController.onEnemyAttackEventPath, enemyController.transform.position);

        enemyController.StopAllCoroutines();
        enemyController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        
        enemyController.animator.SetBool(Animator.StringToHash("Attack"), true);
        enemyController.attackRoutine = enemyController.StartCoroutine(DealDamage(enemyController.enemyData.attackSpeed));
    }

    public override void OnExit()
    {
        enemyController.animator.SetBool(Animator.StringToHash("Attack"), false);
        Debug.Log("Exiting Attack State");
    }
}