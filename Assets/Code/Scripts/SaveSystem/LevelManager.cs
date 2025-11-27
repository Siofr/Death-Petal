using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public LevelSaveData levelSaveData = new LevelSaveData();

    public HashSet<ISaveable> _saveables = new HashSet<ISaveable>();
    public HashSet<ISaveable> _defaultSaveables = new HashSet<ISaveable>();
    
    public void SaveLevelData(ref HashSet<ISaveable> refSaveables)
    {
        if (_defaultSaveables.Count < 1)
        {
            var tempSaveables = FindSaveables();

            if (tempSaveables.Count < 1)
            {
                Debug.Log("No Saveable Objects in Level");
                return;
            }
                
            Debug.Log("Baking Level Data");
            refSaveables = _defaultSaveables;
            _defaultSaveables = tempSaveables.ToHashSet();
        }

        foreach (var saveable in refSaveables)
        {
            saveable.HandleSaveData(ref levelSaveData);
        }
    }

    public void LoadLevelData(ref HashSet<ISaveable> refSaveables)
    {
        if (refSaveables.Count < 1)
        {
            if (_defaultSaveables.Count < 1)
            {
                Debug.Log("No Baked Level Data Found");
                return;
            }
            
            Debug.Log("Loading Default Level Data");
            refSaveables = _defaultSaveables;
        }

        foreach (var saveable in refSaveables)
        {
            saveable.HandleLoadData(ref levelSaveData);
        }
    }

    public void BakeLevelData()
    {
        SaveLevelData(ref _defaultSaveables);
        Debug.Log("Baked Default Level Data");
    }

    public void LoadDefaultData()
    {
        LoadLevelData(ref _defaultSaveables);
        Debug.Log("Loaded Default Level Data");
    }
    
    public List<ISaveable> FindSaveables()
    {
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var tempSaveables = new List<ISaveable>();
        
        foreach (var gameObj in sceneObjects)
        {
            if(!gameObj.TryGetComponent(out ISaveable saveable)) continue;
            
            if(saveable.SaveID == 0) saveable.CreateSaveInstance();
            
            levelSaveData.saveableID.Add(saveable.SaveID);
            
            tempSaveables.Add(saveable);
        }
        
        if(tempSaveables.Count < 1) Debug.Log("No Saveable Objects in Level");
        
        return tempSaveables;
    }

    public void ClearData()
    {
        foreach (var saveable in _defaultSaveables)
        {
            saveable.DeleteSaveInstance();
        }

        foreach (var saveable in _saveables)
        {
            saveable.DeleteSaveInstance();
        }
        
        levelSaveData =  new LevelSaveData();
        _saveables.Clear();
        _defaultSaveables.Clear();
        
        Debug.Log("Cleared Saveable Objects");
    }
}

