public class EnemyLurkerFreezeState : EnemyBaseState<EnemyLurker>
{
    public EnemyLurkerFreezeState(EnemyLurker enemyController) : base(enemyController) { }

    public override void OnEnter()
    {
        enemyController.ToggleFreeze(true);
    }

    public override void OnExit()
    {
        enemyController.ToggleFreeze(false);
    }
}