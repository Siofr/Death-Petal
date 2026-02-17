using System;
using UnityEngine;

public struct SpawnEnemyEvent : IEvent
{
    public Vector3 position;
    public List
    public Type entityType;

    public SpawnEnemyEvent(Vector3 pos, Type type)
    {
        position = pos;
        entityType = type;
    }
}

public class EnemyFactory<T> : MonoBehaviour where T: EnemyBase
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyConfig_SO config;
    
    //Event Fields
    private EventBindings<SpawnEnemyEvent> _spawnEnemyEventListener;
    
    public void CreateEnemy(Vector3 position, WeakTypes[] weaknesses)
    {
        var enemyObj = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
        var enemyController = enemyObj.GetComponent<EnemyBase>();
        enemyController.Initialise(config, weaknesses);
    }

    private void OnSpawnEnemy(SpawnEnemyEvent context)
    {
        if (context.entityType != typeof(T)) return;
        
        CreateEnemy(context.position,);
    }
    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
