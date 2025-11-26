using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public LevelSaveData levelSaveData;

    private HashSet<ISaveable> _saveables = new HashSet<ISaveable>();
    private HashSet<ISaveable> _defaultSaveables = new HashSet<ISaveable>();
    
    public void SaveLevelData()
    {
    }

    public void LoadLevelData()
    {
        
    }

    [ItemCanBeNull]
    public List<ISaveable> FindSaveables()
    {
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var tempSaveables = new List<ISaveable>();
        
        foreach (var gameObj in sceneObjects)
        {
            if(!gameObj.TryGetComponent(out ISaveable saveable)) continue;
            
            tempSaveables.Add(saveable);
        }
        
        if(tempSaveables.Count < 1) Debug.Log("No Saveable Objects in Level");
        
        return tempSaveables;
    }
}

