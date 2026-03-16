using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("LevelManager Fields, Attach Component to Level Prefab")]
    public LevelSaveableData_SO saveableData;
    public LevelSaveData levelSaveData;

    public List<ISaveable> saveables = new List<ISaveable>();
    
    public void SaveLevelData(bool isBaking = false)
    {
        var temp = new LevelSaveData(SceneManager.GetActiveScene().name);
        
        var tempSaveables = FindSaveables();

        if (tempSaveables.Count < 1)
        {
            Debug.Log("No Saveable Objects in Level");
            return;
        }
            
        saveables = tempSaveables;
        var tempIDs = new List<int>();
        
        foreach (var saveable in saveables)
        {
            tempIDs.Add(saveable.SaveID);
            //saveable.HandleSaveData(ref temp);
        }

        temp.saveableID = tempIDs;
        
        foreach (var saveable in saveables)
        {
            saveable.HandleSaveData(ref temp);
        }

        levelSaveData = temp;
        
        if (isBaking)
        {
            var defaultSave = new LevelSaveData(temp.levelName += " Default", temp);
            SaveSystem.SaveLevelData(defaultSave);
            Debug.Log("Baked Level Data");
            return;
        }
        
        SaveSystem.SaveLevelData(temp);
        
        Debug.Log("Saved Level Data");
    }

    public void LoadLevelData(bool isDefault = false)
    {
        var tempName = SceneManager.GetActiveScene().name;
        if(isDefault) tempName += " Default";
        
        var temp = SaveSystem.GetLevelData(tempName);
        
        if (temp.levelName != SceneManager.GetActiveScene().name && temp.levelName != SceneManager.GetActiveScene().name + " Default") return;

        levelSaveData = temp;
        
        var tempSaveables = FindSaveables();
        
        foreach (var saveable in tempSaveables)
        {
            saveable.HandleLoadData(ref levelSaveData);
        }

        if (tempName == SceneManager.GetActiveScene().name+" Default")
        {
            Debug.Log("Loaded Default Level Data");
            return;
        }
        
        Debug.Log("Loaded Level Data");
    }
    
    public List<ISaveable> FindSaveables()
    {
        if (saveableData == null)
        {
            Debug.Log("Please Create a Level Saveable Data Sciptable Object Asset And Attach To Component");
        }
        
        var sceneObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var tempSaveables = new List<ISaveable>();
        
        foreach (var gameObj in sceneObjects)
        {
            if (!gameObj.TryGetComponent(out ISaveable saveable)) continue;
            
            if(saveable.SaveSO == null || saveable.SaveSO.saveID == 0) saveable.CreateSaveInstance(saveableData);
            
            tempSaveables.Add(saveable);
        }
        
        if(tempSaveables.Count < 1) Debug.Log("No Saveable Objects in Level");
        
        return tempSaveables;
    }

    public void ClearData()
    {
        foreach (var saveable in saveables)
        {
            saveable.DeleteSaveInstance(saveableData);
        }
        
        levelSaveData =  new LevelSaveData(SceneManager.GetActiveScene().name);
        saveables.Clear();
        
        Debug.Log("Cleared Saveable Objects");
        
        SaveSystem.RemoveLevelData(SceneManager.GetActiveScene().name);
        
        ISaveableHelper.RemoveAllIDs(saveableData);
    }

    public void Start()
    {
        //if(saveables.Count < 1) saveables = FindSaveables();
        //LoadLevelData();
    }
}
