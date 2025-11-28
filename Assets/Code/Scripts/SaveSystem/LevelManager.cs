using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("LevelManager Fields, Attach Component to Level Prefab")]
    public LevelSaveData levelSaveData;

    public List<ISaveable> saveables = new List<ISaveable>();
    
    public void SaveLevelData(bool isBaking = false)
    {
        if (saveables.Count < 1)
        {
            var tempSaveables = FindSaveables();
            
            if (tempSaveables.Count < 1) return;
            
            saveables = tempSaveables;
            var tempIDs = new List<int>();
            
            foreach (var saveable in saveables)
            {
                tempIDs.Add(saveable.SaveID);
            }

            levelSaveData.saveableID = tempIDs;
        }
        
        foreach (var saveable in saveables)
        {
            saveable.HandleSaveData(ref levelSaveData);
        }
        
        if (isBaking)
        {
            var defaultSave = new LevelSaveData(transform.name + "Default", levelSaveData);
            
            SaveSystem.SaveLevelData(defaultSave);
            Debug.Log("Baked Level Data");
            return;
        }

        levelSaveData.levelName = transform.name;
        
        SaveSystem.SaveLevelData(levelSaveData);
        
        Debug.Log("Saved Level Data");
    }

    public void LoadLevelData(bool isDefault = false)
    {
        var tempName = transform.name;
        if(isDefault) tempName = transform.name + "Default";
        
        var temp = SaveSystem.GetLevelData(tempName);
        
        if (temp.levelName != transform.name && temp.levelName != transform.name + "Default") return;

        levelSaveData = temp;
        
        foreach (var saveable in saveables)
        {
            saveable.HandleLoadData(ref levelSaveData);
        }

        if (tempName == transform.name+"Default")
        {
            Debug.Log("Loaded Default Level Data");
            return;
        }
        
        Debug.Log("Loaded Level Data");
    }
    
    public List<ISaveable> FindSaveables()
    {
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var tempSaveables = new List<ISaveable>();
        
        foreach (var gameObj in sceneObjects)
        {
            if (!gameObj.TryGetComponent(out ISaveable saveable)) continue;
            
            if(saveable.SaveID == 0) saveable.CreateSaveInstance();
            
            tempSaveables.Add(saveable);
        }
        
        if(tempSaveables.Count < 1) Debug.Log("No Saveable Objects in Level");
        
        return tempSaveables;
    }

    public void ClearData()
    {
        foreach (var saveable in saveables)
        {
            saveable.DeleteSaveInstance();
        }
        
        levelSaveData =  new LevelSaveData(transform.name);
        saveables.Clear();
        
        Debug.Log("Cleared Saveable Objects");
        
        SaveSystem.RemoveLevelData(transform.name);
    }

    public void Start()
    {
        if(saveables.Count < 1) saveables = FindSaveables();
        LoadLevelData();
    }
}

