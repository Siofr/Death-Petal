using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Data Fields")] 
    [SerializeField] private string _dataAssetSavePath;
    
    //Non-Serializable Fields
    public Dictionary<ISaveable, SaveData> saveables = new Dictionary<ISaveable, SaveData>();
    public Dictionary<ISaveable, SaveData> defaultSaveables = new Dictionary<ISaveable, SaveData>();
    
    //Properties
    public string AssetSavePath => _dataAssetSavePath;
    public Dictionary<ISaveable, SaveData> Saveables => saveables;
    public Dictionary<ISaveable, SaveData> DefaultSaveables => defaultSaveables;
    
    public void SaveLevelData(Dictionary<ISaveable, SaveData> saveRef)
    {
        var tempSaveables = FindSaveables();

        if (tempSaveables.Count < 1)
        {
            Debug.Log("No Saveables in Level");
            return;
        }

        foreach (var saveable in tempSaveables)
        {
            saveable.Key.SaveData();
        }

        Debug.Log($"Saved {tempSaveables} to Level Data");
        saveRef = tempSaveables;
    }   

    public void LoadLevelData(Dictionary<ISaveable, SaveData> loadRef)
    {
        var tempSaveables = loadRef;
        
        if (tempSaveables.Count < 1)
        {
            if (DefaultSaveables.Count < 1)
            {
                Debug.Log("No Baked Save Data");
                return;
            }

            tempSaveables = DefaultSaveables;
        }
        
        foreach (var saveable in tempSaveables)
        {
            saveable.Key.LoadSaveData(saveable.Value);
        }
        
        Debug.Log("Loaded Save Data");
    }

    public void ClearLevelData()
    {
        defaultSaveables.Clear();   
        saveables.Clear();
        
        Debug.Log("Cleared Level Data");
    }

    public Dictionary<ISaveable, SaveData> FindSaveables()
    {
        var result = new Dictionary<ISaveable, SaveData>();

        var gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var @object in gameObjects)
        {
            if (@object.TryGetComponent(out ISaveable saveable))
            {
                result.Add(saveable, saveable.GetSaveData(this));           
            }
        }

        return result;
    }
}

