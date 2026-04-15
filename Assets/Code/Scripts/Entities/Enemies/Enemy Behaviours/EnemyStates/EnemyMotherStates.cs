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
        _mother.animator.SetBool("Vulnerable", true);
        _mother.wingMaterial.SetFloat("_Lerp", 1f);
    }

    // public override void Update()
    // {
    //     _mother.LookAtTarget();
    // }
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
        _mother.animator.SetBool("Vulnerable", false);
        _mother.wingMaterial.SetFloat("_Lerp", 0f);
    }
    
    public override void Update()
    {
        //_mother.LookAtTarget();
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
        //_mother.LookAtTarget();
        _mother.CheckSpawnedEnemies();
    }
}