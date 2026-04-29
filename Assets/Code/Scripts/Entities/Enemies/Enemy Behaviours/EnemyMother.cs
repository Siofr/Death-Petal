using System;
using System.Collections;
using System.Collections.Generic;
using State_Machine;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyType
{
    Hellspawn,
    Lurker,
    Mother
}

public class EnemyMother: EnemyBase
{
    [Header("Mother Fields")] 
    [SerializeField] private List<EnemyBase> _spawnedEnemies = new List<EnemyBase>();
    [SerializeField] private Transform _spawnPointRef;
    [SerializeField] public Material wingMaterial;
    [SerializeField] private EnemyType _spawnType;
    public float maxSpawnCount;
    public float spawnTime;
    
    //Non-Serialized Fields
    private Coroutine _spawnRoutine;
    private bool _isFirstEncounter;
    
    //Event Fields
    private EventBindings<SpawnedEnemyEvent> _onSpawnedEnemiesListener;
    
    protected override void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyMother>(this);
        var deathState = new EnemyDeathState<EnemyMother>(this);
        var vulnerableState = new EnemyMotherVulnerableState(this);
        var protectedState = new EnemyMotherProtectedState(this);
        var fullState = new EnemyMotherFullState(this);
        
        __enemyStateMachine.AddAnyTransition(idleState, new FuncPredicate( ()=> target == null));
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( ()=> IsDead));
        __enemyStateMachine.AddAnyTransition(fullState, new FuncPredicate( ()=> _spawnedEnemies.Count >= maxSpawnCount));
        __enemyStateMachine.AddAnyTransition(vulnerableState, new FuncPredicate(()=> _spawnedEnemies.Count <1));
        __enemyStateMachine.AddAnyTransition(protectedState, new FuncPredicate(()=> _spawnedEnemies.Count < maxSpawnCount ));
        
        __enemyStateMachine.SetState(idleState);
    }

    protected override void Initialise()
    {
        base.Initialise();

        var wingColor = _spawnType switch
        {
            EnemyType.Lurker => Color.crimson,
            EnemyType.Hellspawn => Color.peru,
            _ => Color.aquamarine
        };
        
        wingMaterial.SetColor("_WingColor", wingColor);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        __playerRoomEnterEventListener.Add(CheckFirstEncounter);
        _onSpawnedEnemiesListener = new EventBindings<SpawnedEnemyEvent>(OnSpawnedEnemy);
        
        EventBus<SpawnedEnemyEvent>.Register(_onSpawnedEnemiesListener);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        EventBus<SpawnedEnemyEvent>.Unregister(_onSpawnedEnemiesListener);
    }
    
    /*public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        //TO REMOVE JUST FOR TESTING
        if (Weaknesses.Count == 1 && Weaknesses[0].WeakType == damageType)
        {
            Destroy(gameObject);
        }
        
        base.OnShot(weakness, damageType);
        
        print("Mother Enemy Shot");
    }*/

    private void CheckFirstEncounter(RoomPlayerEnterEvent context)
    {
        if (context.room.transform != transform.parent) return;

        if (!_isFirstEncounter)
        {
            _isFirstEncounter = true;
            
            __playerRoomEnterEventListener.Remove(CheckFirstEncounter);
        }
    }
    
    public void SpawnEnemy(float spawnTime)
    {
        if (_spawnRoutine != null) return;
        
        Debug.Log("Starting Goon Spawn");
        
        _spawnRoutine = StartCoroutine(SpawnRoutine(spawnTime));
    }

    public void CheckSpawnedEnemies()
    {
        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if(_spawnedEnemies[i] == null || _spawnedEnemies[i].IsDead) _spawnedEnemies.RemoveAt(i);
        }

        if (_spawnedEnemies.Count < maxSpawnCount)
        {
            SpawnEnemy(spawnTime);
        }
    }
    
    private IEnumerator SpawnRoutine(float spawnTimeInterval)
    {
        while (_spawnedEnemies.Count < maxSpawnCount)
        {
            if (!_isFirstEncounter)
            {
                yield return new WaitForSeconds(spawnTimeInterval);
            }

            if (__isPaused) yield return null;
            
            var enemyType = _spawnType switch
            {
                EnemyType.Lurker => typeof(EnemyLurker),
                EnemyType.Mother => typeof(EnemyMother),
                _ => typeof(EnemyBase)
            };
            
            if (__isPaused) yield return null;
            
            EventBus<SpawnEnemyEvent>.Raise(new SpawnEnemyEvent(_spawnPointRef.position, enemyType, transform.parent, gameObject));
            
            if(_isFirstEncounter) _isFirstEncounter = false;
            
            yield return null;
        }

        _spawnRoutine = null;
    }

    public override void StopAllStateRoutines()
    {
        base.StopAllStateRoutines();
        
        _spawnRoutine = null;
        
        Debug.Log("Ending All Mother Routines");
    }

    private void OnSpawnedEnemy(SpawnedEnemyEvent context)
    {
        Debug.Log("Enemy Spawned");
        Debug.Log($"Request: {context.requestObj.name}, Mother: {gameObject.name}");
        
        if (context.requestObj != gameObject) return;
        
        _spawnedEnemies.Add(context.enemy);
    }

    protected override void Update()
    {
        base.Update();
        //print(__enemyStateMachine.GetActiveState());
    }
}