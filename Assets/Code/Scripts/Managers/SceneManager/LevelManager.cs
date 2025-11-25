using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct SaveEvent: IEvent { }

public struct LoadEvent : IEvent
{
    public bool isDefault;
    
    public LoadEvent(bool value) => isDefault = value;
}

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private LevelData _levelData;
    
    //Events
    private EventBindings<SaveEvent> _saveEventListener;
    private EventBindings<LoadEvent> _loadEventListener;
    
    private void OnEnable()
    {
        _saveEventListener = new EventBindings<SaveEvent>(OnSave);
        _loadEventListener = new EventBindings<LoadEvent>(OnLoad);
        
        EventBus<SaveEvent>.Register(_saveEventListener);
        EventBus<LoadEvent>.Register(_loadEventListener);
    }

    private void OnDisable()
    {
        EventBus<SaveEvent>.Unregister(_saveEventListener);
        EventBus<LoadEvent>.Unregister(_loadEventListener);
    }
    
    private void OnSave()
    {
    }

    private void OnLoad(LoadEvent context)
    {
    }

    // private void Start()
    // {
    //     _levelData.LoadLevelData(_levelData.defaultSaveables);
    // }
}