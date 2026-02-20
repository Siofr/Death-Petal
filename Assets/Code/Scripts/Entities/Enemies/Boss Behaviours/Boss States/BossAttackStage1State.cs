using System.Collections;
using System.Drawing.Printing;
using FMODUnity;
using UnityEngine;

public enum Bishop_Phase1Attacks
{
    TargetSpit,
    RadialSpit,
    SummonBackup
}

public class BossAttackStage1State : BossBaseState
{
    

    private Bishop_Phase1Attacks _activeAttack;

    private int _previousAttackValue = -1;

    public BossAttackStage1State(BossBase bossController) : base(bossController) { }
    
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

        RuntimeManager.PlayOneShot(bossController.onEnemyAttackEventPath, bossController.transform.position);

        bossController.StopAllCoroutines();
        bossController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        
        bossController.animator.SetBool(Animator.StringToHash("Attack"), true);
        bossController.attackRoutine = bossController.StartCoroutine(DealDamage(bossController.enemyData.attackSpeed));
    }

    public void AttackSelector()
    {
        int randSelection = Random.Range(0, 2);
        if(randSelection == _previousAttackValue) randSelection = Random.Range(0, 2);

        _previousAttackValue = randSelection;

        switch (randSelection)
        {
            case 0:
                _activeAttack = Bishop_Phase1Attacks.TargetSpit;
                break;
            case 1:
                _activeAttack = Bishop_Phase1Attacks.RadialSpit;
                break;
            case 2:
                _activeAttack = Bishop_Phase1Attacks.SummonBackup;
                break;
        }
    }
    


    public override void OnExit()
    {
        bossController.animator.SetBool(Animator.StringToHash("Attack"), false);
        Debug.Log("Exiting Attack State");
    }
}