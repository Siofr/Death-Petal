using UnityEngine;
using FMODUnity;

public class EnemyArcherTargetState: EnemyBaseState<EnemyArcher>
{
    public EnemyArcherTargetState(EnemyArcher enemyController) : base(enemyController)
    {
        _archer = enemyController;
    }

    private EnemyArcher _archer;  
    
    public override void OnEnter()
    {
        RuntimeManager.PlayOneShot(enemyController.onArrowKnock, enemyController.transform.position);
        _archer.ToggleLineRenderer(true);
        _archer.ToggleLineRendererColor(Color.white);
        _archer.StartTargeting(_archer.targetTime);
        
        Debug.Log("Enter Target State");
    }

    public override void OnExit()
    {
        _archer.ToggleLineRenderer(false);
        _archer.StopAllStateRoutines();
    }

    public override void Update()
    {
        //_archer.CheckLOS(_archer.maxLOSRadius, _archer.enemyData.attackRange);
        _archer.UpdateLineRenderer();
    }
}

public class EnemyArcherShootState : EnemyBaseState<EnemyArcher>
{
    public EnemyArcherShootState(EnemyArcher enemyController) : base(enemyController)
    {
        _archer = enemyController;
    }
    
    private EnemyArcher _archer;

    public override void OnEnter()
    {
        RuntimeManager.PlayOneShot(enemyController.onArrowRelease, enemyController.transform.position);
        _archer.ToggleLineRenderer(true);
        _archer.ToggleLineRendererColor(Color.red);
        _archer.StartShot(_archer.enemyData.attackSpeed);
        
        Debug.Log("Enter Shoot State");
    }
    
    public override void OnExit()
    {
        _archer.StopAllStateRoutines();
        _archer.ToggleLineRenderer(false);
    }
}

public class EnemyArcherAlertState : EnemyBaseState<EnemyArcher>
{
    public EnemyArcherAlertState(EnemyArcher enemyController) : base(enemyController)
    {
        _archer = enemyController;
    }
    
    private EnemyArcher _archer;

    public override void Update()
    {
        _archer.CheckLOS(_archer.maxLOSRadius, _archer.enemyData.attackRange);
    }
    
    public override void OnEnter()
    {
        _archer.StartAlertRoutine(2f, _archer.alertRotationAngle, 60f);
        
        Debug.Log("Enter Alert State");
    }
    
    public override void OnExit()
    {
        _archer.StopAllStateRoutines();
    }
}