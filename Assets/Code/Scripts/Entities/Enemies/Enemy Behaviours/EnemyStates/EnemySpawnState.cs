public class EnemySpawnState<T>: EnemyBaseState<T> where T: EnemyBase
{
    public EnemySpawnState(T enemyController) : base(enemyController) { }

    public override void OnEnter()
    {
        enemyController.Weaknesses[0].Toggle(false);
        enemyController.StopAgent(true);
    }

    public override void OnExit()
    {
        enemyController.Weaknesses[0].Toggle(true);
        enemyController.StopAgent(false);
    }
}