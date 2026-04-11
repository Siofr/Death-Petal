using UnityEngine;

public class EnemyMotherVulnerableState: EnemyBaseState<EnemyMother>
{
    private EnemyMother _mother;
    
    public EnemyMotherVulnerableState(EnemyMother enemyController) : base(enemyController)
    {
        _mother = enemyController;
    }

    public override void OnEnter()
    {
        Debug.Log("Mother Vulnerable");
        
        _mother.SpawnEnemy(_mother.spawnTime);
        _mother.Weaknesses[0].Toggle(true);
    }

    public override void Update()
    {
        _mother.LookAtTarget();
    }
}

public class EnemyMotherProtectedState: EnemyBaseState<EnemyMother>
{
    private EnemyMother _mother;
    
    public EnemyMotherProtectedState(EnemyMother enemyController) : base(enemyController)
    {
        _mother = enemyController;
    }

    public override void OnEnter()
    {
        Debug.Log("Mother Protected");
        _mother.SpawnEnemy(_mother.spawnTime);
        _mother.Weaknesses[0].Toggle(false);
    }
    
    public override void Update()
    {
        _mother.LookAtTarget();
        _mother.CheckSpawnedEnemies();
    }
}

public class EnemyMotherFullState: EnemyBaseState<EnemyMother>
{
    private EnemyMother _mother;
    
    public EnemyMotherFullState(EnemyMother enemyController) : base(enemyController)
    {
        _mother = enemyController;
    }

    public override void OnEnter()
    {
        Debug.Log("Mother Full");
        _mother.Weaknesses[0].SetWeakType(WeakTypes.PLAYER);
    }
    
    public override void Update()
    {
        _mother.LookAtTarget();
        _mother.CheckSpawnedEnemies();
    }
}