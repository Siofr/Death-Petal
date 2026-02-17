
using UnityEngine;

public class EnemyArcherTargetState: EnemyBaseState
{
    public EnemyArcherTargetState(EnemyArcher enemyController) : base(enemyController)
    {
        _archer = enemyController;
    }

    private EnemyArcher _archer;  
    
    public override void OnEnter()
    {
        _archer.ToggleLineRenderer(true);
        _archer.StartTargeting(_archer.targetTime);
        
        Debug.Log("Enter Target State");
    }

    public override void OnExit()
    {
        _archer.ToggleLineRenderer(false);
        _archer.StopAllArcherRoutines();
    }
}

public class EnemyArcherShootState : EnemyBaseState
{
    public EnemyArcherShootState(EnemyArcher enemyController) : base(enemyController)
    {
        _archer = enemyController;
    }
    
    private EnemyArcher _archer;

    public override void OnEnter()
    {
        _archer.StartShot(_archer.enemyData.attackSpeed);
        
        Debug.Log("Enter Shoot State");
    }
    
    public override void OnExit()
    {
        _archer.StopAllArcherRoutines();
    }
}

public class EnemyArcherAlertState : EnemyBaseState
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
        _archer.StopAllArcherRoutines();
    }
}