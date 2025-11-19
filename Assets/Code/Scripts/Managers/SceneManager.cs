using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct LevelLoadingEvent : IEvent
{
}

public struct LevelLoadedEvent : IEvent
{
    
}

public struct LevelDeloadingEvent : IEvent
{
    
}

public struct LevelDeloadedEvent : IEvent
{
    
}

public class SceneManager : Singleton<SceneManager>
{
    [Header("Levels")] [SerializeField] private Transform _levelContainer;
    [SerializeField] private GameObject[] _levelPrefabs;

    [Header("Managers")] [SerializeField] private Transform _managerContainer;
    [SerializeField] private GameObject[] _managerPrefabs;

    [Header("UI")] [SerializeField] private Transform _uiContainer;
    [SerializeField] private GameObject[] _uiPrefabs;

    [Header("Player")] [SerializeField] private Transform _playerContainer;
    [SerializeField] private GameObject _playerPrefab;

    //Non-Serializable Fields
    private int _currentLevelID;

    //Properties
    public GameObject[] Levels => _levelPrefabs;
    public GameObject[] Managers => _managerPrefabs;
    public GameObject[] UI => _uiPrefabs;

    public async Task LoadLevel(int id)
    {
        
    }

    public async Task LoadUI(int id)
    {
        
    }

    public async Task LoadUI(string name)
    {
        for (int i = 0; i < _uiPrefabs.Length; i++)
        {
            if(_uiPrefabs[i].name == name)
            {
                await LoadUI(i);
                return;
            }
        }
    }
    
    public async Task LoadManager(int id)
    {
    }

    public async Task UnloadLevel(int id)
    {
        
    }

    public async Task ReloadLevel()
    {
        await LoadLevel(_currentLevelID);
    }
}

public abstract class EntitySaveData_SO: ScriptableObject { }
