using System.Collections;
using FMODUnity;
using UnityEngine;

public class BossAttackStage2State : BossBaseState
{
    public BossAttackStage2State(BossBase bossController) : base(bossController) { }
    
    private IEnumerator DealDamage(float attackSpeed)
    {
        IEntity playerEntity;
        
        bossController.target.TryGetComponent(out playerEntity);
        
        yield return new WaitForSeconds(attackSpeed);
        
        while (playerEntity.Weaknesses.Count > 0)
        {
            playerEntity.OnShot(playerEntity.Weaknesses[0], WeakTypes.PLAYER);
            Debug.Log("Damage Dealt to Player");
            
            yield return new WaitForSeconds(attackSpeed);

            if (!bossController.InAttackRange()) break;
        }
        
        Debug.Log("Attack Phase Over");
        
        bossController.attackRoutine = null;
    }
    
    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        //enemyController.SetTarget(null);

        RuntimeManager.PlayOneShot(bossController.onEnemyAttackEventPath, bossController.transform.position);

        bossController.StopAllCoroutines();
        bossController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        
        bossController.animator.SetBool(Animator.StringToHash("Attack"), true);
        bossController.attackRoutine = bossController.StartCoroutine(DealDamage(bossController.enemyData.attackSpeed));
    }

    public override void OnExit()
    {
        bossController.animator.SetBool(Animator.StringToHash("Attack"), false);
        Debug.Log("Exiting Attack State");
    }
}
