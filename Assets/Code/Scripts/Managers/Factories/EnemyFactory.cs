using UnityEngine;

public struct SpawnEnemyEvent : IEvent
{
    public Vector3 position;
    public EnemyFactory 
    
    public SpawnEnemyEvent()
    {
        
    }
}

public class EnemyFactory<T> : MonoBehaviour where T: EnemyBase
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyConfig_SO config;
    
    public void CreateEnemy(Vector3 position, WeakTypes[] weaknesses)
    {
        var enemyObj = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
        var enemyController = enemyObj.GetComponent<EnemyBase>();
        enemyController.Initialise(config, weaknesses);
    }
}
