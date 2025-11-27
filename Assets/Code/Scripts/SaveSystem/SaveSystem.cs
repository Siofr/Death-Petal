using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    public static GameSaveData gameSaveData = GameSaveData.Initialise();
    
    public static string GetSaveDataPath()
    {
        Debug.Log(Application.persistentDataPath + "/Save.dps");
        return Application.persistentDataPath + "/Save.dps";
    }

    public static void SaveGameData()
    {
        JsonUtility.ToJson(gameSaveData);
        File.WriteAllText(GetSaveDataPath(), JsonUtility.ToJson(gameSaveData, true));
        Debug.Log("Saved Game Data");
    }

    public static void LoadGameData()
    {
        gameSaveData = JsonUtility.FromJson<GameSaveData>(File.ReadAllText(GetSaveDataPath()));
        Debug.Log("Read Game Save Data");
    }
    
    public static void SaveLevelData(LevelSaveData levelSaveData)
    {
        for (int i = 0; i < gameSaveData.levelSaveData.Count; i++)
        {
            if (gameSaveData.levelSaveData[i].levelName != levelSaveData.levelName) continue;
            
            gameSaveData.levelSaveData[i] = levelSaveData;
            SaveGameData();
            return;
        }
        
        gameSaveData.levelSaveData.Add(levelSaveData);
        SaveGameData();
    }

    public static LevelSaveData GetLevelData(string levelName)
    {
        LoadGameData();
        
        var defaultDataName = levelName + "Default";
        var defaultDataTemp = new LevelSaveData();
        
        for (int i = 0; i < gameSaveData.levelSaveData.Count; i++)
        {
            if (gameSaveData.levelSaveData[i].levelName == levelName)
            {
                Debug.Log($"Found {levelName} Data");
                return gameSaveData.levelSaveData[i];
            }
            
            if(gameSaveData.levelSaveData[i].levelName != defaultDataName) continue;
            defaultDataTemp = gameSaveData.levelSaveData[i];
        }
        
        if(defaultDataTemp.levelName == defaultDataName) Debug.Log($"Found Default {levelName} Data");
        else Debug.Log($"No {defaultDataName} Data Found");
        
        return defaultDataTemp;
    }

    public static void RemoveLevelData(string levelName)
    {
        var count = 0;
        for (var i = gameSaveData.levelSaveData.Count - 1; i >= 0; i--)
        {
            if (gameSaveData.levelSaveData[i].levelName != levelName &&
                gameSaveData.levelSaveData[i].levelName != levelName + "Default") continue;
            
            gameSaveData.levelSaveData.RemoveAt(i);
            count++;
            if (count > 1) return;
        }
        
        SaveGameData();
        
        Debug.Log($"Removed {levelName} Data");
    }
}

#region Save Data Types
[Serializable]
public struct GameSaveData
{
    public List<LevelSaveData> levelSaveData;

    static public GameSaveData Initialise()
    {
        var result = new GameSaveData();
        result.levelSaveData = new List<LevelSaveData>();

        return result;
    }
}

[Serializable]
public struct LevelSaveData
{
    public string levelName;
    public List<int> saveableID;
    
    public List<EntitySaveData> entitySaveData;
    public List<PuzzleOutputSaveData> puzzleOutputSaveData;
    public List<PuzzleElementSaveData> puzzleElementSaveData;

    public LevelSaveData(string levelName)
    {
        this.levelName = levelName;
        saveableID = new List<int>();
        entitySaveData = new List<EntitySaveData>();
        puzzleOutputSaveData = new List<PuzzleOutputSaveData>();
        puzzleElementSaveData = new List<PuzzleElementSaveData>();
    }

    public LevelSaveData(string levelName, LevelSaveData levelSaveData)
    {
        this = levelSaveData;
        this.levelName = levelName;
    }
}
#endregion

#region ISaveable Save Data Types
[Serializable]
public struct EntitySaveData
{
    public int id;
    
    public Vector3 position;
    public List<int> health;

    public EntitySaveData(int id, Vector3 position,  List<int> health)
    {
        this.id = id;
        this.position = position;
        this.health = health;
    }
    
    public void Save(Vector3 pos, List<Weakness> refWeaknesses)
    {
        position = pos;

        var tempWeaknesses = new List<int>();
        refWeaknesses.ForEach((x)=> tempWeaknesses.Add((int)x.WeakType));

        health = tempWeaknesses;
    }

    public void Load(Transform refTransform, ref List<Weakness> refWeaknesses)
    {
        refTransform.position = position;

        if (health.Count < 1)
        {
            refWeaknesses.Clear();
        }
    }
}

[Serializable]
public struct PuzzleOutputSaveData
{
    public Vector3 position;
    public bool isSolved;
    
    public void Save(Vector3 pos, bool solvedCondition)
    {
        position = pos;
        isSolved = solvedCondition;
    }

    public void Load(Transform refTransform, ref bool refSolvedCondition)
    {
        refTransform.position = position;
        refSolvedCondition = isSolved;
    }
}

[Serializable]
public struct PuzzleElementSaveData
{
    public Vector3 position;
    
    public void Save(Vector3 pos)
    {
        position = pos;
    }

    public void Load(Transform refTransform)
    {
        refTransform.position = position;
    }
}
#endregion