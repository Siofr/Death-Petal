
public class EnemyArcherTargetState: EnemyBaseState
{
    public EnemyArcherTargetState(EnemyBase enemyController) : base(enemyController) { }

    private EnemyArcher _archer;

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
        
    }
}

public class EnemyArcherShootState : EnemyBaseState
{
    public EnemyArcherShootState(EnemyBase enemyController) : base(enemyController) { }
}

public class EnemyArcherAlertState : EnemyBaseState
{
    public EnemyArcherAlertState(EnemyBase enemyController) : base(enemyController) { }
}