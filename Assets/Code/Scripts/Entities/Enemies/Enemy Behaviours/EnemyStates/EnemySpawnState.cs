public class EnemySpawnState<T>: EnemyBaseState<T> where T: EnemyBase
{
    public EnemySpawnState(T enemyController) : base(enemyController) { }

    public override void OnEnter()
    {
        enemyController.StopAgent(true);
    }

    public override void OnExit()
    {
        enemyController.StopAgent(false);
    }
}