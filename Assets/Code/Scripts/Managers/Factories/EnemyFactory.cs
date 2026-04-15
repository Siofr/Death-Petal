using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public struct SpawnEnemyEvent : IEvent
{
    public Vector3 position;
    public Type entityType;
    public Transform roomTransform;
    [CanBeNull] public GameObject requestObj;
    
    public SpawnEnemyEvent(Vector3 pos, Type type, Transform room, GameObject obj = null)
    {
        position = pos;
        entityType = type;
        roomTransform = room;
        requestObj = obj;
    }
}

public struct SpawnedEnemyEvent : IEvent
{
    public EnemyBase enemy;
    public GameObject requestObj;

    public SpawnedEnemyEvent(EnemyBase enemy, GameObject requestObj)
    {
        this.enemy = enemy;
        this.requestObj = requestObj;
    }
}

public interface IEnemyFactory<out T> where T: EnemyBase
{
    public Type GetType();
    public void CreateEnemy(Vector3 position, Transform parentRoom, GameObject requestObj = null);
}

public class EnemyFactory<T> : MonoBehaviour, IEnemyFactory<T> where T: EnemyBase
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyConfig_SO config;
    
    //Event Fields
    private EventBindings<SpawnEnemyEvent> _spawnEnemyEventListener;
    
    public Type GetType()
    {
        return typeof(T);
    }

    public void CreateEnemy(Vector3 position, Transform parentRoom, GameObject requestObj = null)
    {
        var enemyObj = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation, parentRoom);
        var enemyController = enemyObj.GetComponent<T>();
        
        enemyController.InitialiseWeaknesses();
        
        var randomNum = new int[enemyController.Weaknesses.Count];
        
        for (int i = 0; i < randomNum.Length; i++)
        {
            randomNum[i] = Random.Range(0, 3);

            var temp = randomNum[i] switch
            {
                0 => WeakTypes.RED,
                1 => WeakTypes.BLUE,
                _ => WeakTypes.GREEN
            };
            
            print(temp);
            
            enemyController.Weaknesses[i].SetWeakType(temp);
        }
        
        enemyController.Initialise(config);
        
        if (requestObj != null)
        {
            EventBus<SpawnedEnemyEvent>.Raise(new SpawnedEnemyEvent(enemyController, requestObj));
        }
    }

    private void OnSpawnEnemy(SpawnEnemyEvent context)
    {
        if (context.entityType != typeof(T)) return;
        
        CreateEnemy(context.position, context.roomTransform, context.requestObj);
    }
    
    private void OnEnable()
    {
        _spawnEnemyEventListener = new EventBindings<SpawnEnemyEvent>(OnSpawnEnemy);
        EventBus<SpawnEnemyEvent>.Register(_spawnEnemyEventListener);
    }

    private void OnDisable()
    {
        EventBus<SpawnEnemyEvent>.Unregister(_spawnEnemyEventListener);
    }
}




