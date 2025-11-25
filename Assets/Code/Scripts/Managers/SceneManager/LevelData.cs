using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct LevelJsonData
{
    public SaveData[] saveData;
    
    public LevelJsonData(SaveData[] saveData)
    {
        this.saveData = saveData;
    }
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    public LevelJsonData saveDataJson;
    
    //Non-Serializable Fields
    public Dictionary<ISaveable, SaveData> saveables = new Dictionary<ISaveable, SaveData>();
    public Dictionary<ISaveable, SaveData> defaultSaveables = new Dictionary<ISaveable, SaveData>();
    
    //Properties
    public Dictionary<ISaveable, SaveData> Saveables => saveables;
    public Dictionary<ISaveable, SaveData> DefaultSaveables => defaultSaveables;
    
    public void SaveLevelData(ref Dictionary<ISaveable, SaveData> refSaveables)
    {
        if (refSaveables.Count < 1)
        {
            var tempSaveables = FindSaveables();
            
            if (tempSaveables.Count < 1)
            {
                Debug.Log("No Saveable Objects in Level");
                return;

            }
            
            refSaveables = tempSaveables;
        }
        
        foreach(var saveable in refSaveables.Keys) saveable.SaveData();
        
        saveDataJson = new LevelJsonData(refSaveables.Values.ToArray());
        
        File.WriteAllText(GetLevelDataPath(), JsonUtility.ToJson(saveDataJson));
        Debug.Log("Saved Level Data");
    }

    public void BakeLevelData()
    {
        SaveLevelData(ref defaultSaveables);
        if(DefaultSaveables.Count > 0) Debug.Log("Baked Level Data");
    }
    
    public void LoadLevelData(ref Dictionary<ISaveable, SaveData> refSaveables)
    {
        if (refSaveables.Count < 0)
        {
            if (defaultSaveables.Count < 1)
            {
                Debug.Log("No Available Level Data");
            }
            
            Debug.Log("Loading Default Level Data");
            refSaveables = DefaultSaveables;
        }
        
        saveDataJson =  JsonUtility.FromJson<LevelJsonData>(File.ReadAllText(GetLevelDataPath()));
        var saveablesArray = refSaveables.Keys.ToArray();

        if (saveablesArray.Length != saveDataJson.saveData.Length)
        {
            Debug.LogError("Loading Level Data Failed");
            return;
        }
        
        refSaveables.Clear();
        
        for (var i = 0; i < saveablesArray.Length; i++)
        {
            saveablesArray[i].LoadData(saveDataJson.saveData[i]);
            refSaveables.Add(saveablesArray[i], saveDataJson.saveData[i]);
        }
        
        Debug.Log("Loaded Level Data");
    }

    public void LoadDefaultLevelData()
    {
        LoadLevelData(ref defaultSaveables);
    }
    
    public string GetLevelDataPath()
    {
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + $"/{name}.lvl";
    }
    
    public Dictionary<ISaveable, SaveData> FindSaveables()
    {
        var results = new Dictionary<ISaveable, SaveData>();
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var gameObject in sceneObjects)
        {
            if(!gameObject.TryGetComponent(out ISaveable saveable)) continue;
            
            results.Add(saveable, saveable.GetSaveInfo());
        }

        return results;
    }
}

