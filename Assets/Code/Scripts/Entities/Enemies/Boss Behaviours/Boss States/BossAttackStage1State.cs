using System.Collections;
using System.Drawing.Printing;
using FMODUnity;
using State_Machine;
using UnityEngine;

public enum Bishop_Phase1Attacks
{
    TargetSpit,
    RadialSpit,
    SummonBackup,
    DualRadialSpit,
    None
}

public class BossAttackStage1State<T> : BossBaseState<T> where T : BossBase
{
    private int _previousAttackValue = -1;

    public BossAttackStage1State(T bossController) : base(bossController) { }
    
    
    public override void OnEnter()
    {
        Debug.Log("Entering Attack 1 State");
        AttackSelector();

        bossController.StopAllCoroutines();
    }

    public void AttackSelector()
    {
        int randSelection = Random.Range(0, 3);
        if(randSelection == _previousAttackValue) randSelection = Random.Range(0, 3);

        _previousAttackValue = randSelection;

        switch (randSelection)
        {
            case 0:
                bossController.activeAttack = Bishop_Phase1Attacks.TargetSpit;
                bossController.attackPatternSpawner.StartTargetSpit();
                break;
            case 1:
                bossController.activeAttack = Bishop_Phase1Attacks.RadialSpit;
                bossController.attackPatternSpawner.StartRadialSpit();
                break;
            case 2:
                bossController.activeAttack = Bishop_Phase1Attacks.DualRadialSpit;
                // Temporarily use dual spit for this
                bossController.attackPatternSpawner.StartRadialSpit(true);
                break;
            case 3:
                bossController.activeAttack = Bishop_Phase1Attacks.SummonBackup;
                // Temporarily use dual spit for this
                bossController.attackPatternSpawner.StartSpawnBackup();
                break;
        }
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Attack State");
    }
}