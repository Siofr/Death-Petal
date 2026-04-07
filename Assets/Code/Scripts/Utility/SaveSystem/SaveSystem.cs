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
        
        var defaultDataName = levelName + " Default";
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
                gameSaveData.levelSaveData[i].levelName != levelName + " Default") continue;
            
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
    
    public LevelSaveData(string levelName)
    {
        this.levelName = levelName;
        saveableID = new List<int>();
        entitySaveData = new List<EntitySaveData>();
        puzzleOutputSaveData = new List<PuzzleOutputSaveData>();
    }
    
    public LevelSaveData(string levelName, LevelSaveData levelSaveData)
    {
        this = levelSaveData;
        this.levelName = levelName;
    }
}

[Serializable]
public struct WeaknessSaveData
{
    public Vector3 iconPosition;
    public int weakType;

    public WeaknessSaveData(Vector3 iconPosition, int weakType)
    {
        this.iconPosition = iconPosition;
        this.weakType = weakType;
    }
}
#endregion

#region ISaveable Save Data Types
[Serializable]
public struct EntitySaveData
{
    public int id;
    
    public Vector3 position;
    public List<WeaknessSaveData> health;

    public EntitySaveData(int id, Vector3 position,  List<WeaknessSaveData> health)
    {
        this.id = id;
        this.position = position;
        this.health = health;
    }
    
    public void Save(Vector3 pos, List<Weakness> refWeaknesses)
    {
        position = pos;

        var tempWeaknesses = new List<WeaknessSaveData>();
        
        for (int i = 0; i < refWeaknesses.Count; i++)
        {
            tempWeaknesses.Add(new WeaknessSaveData(refWeaknesses[i].WeaknessIconTransform.position, (int)refWeaknesses[i].WeakType));
        }

        health = tempWeaknesses;
    }

    public void Load(Transform refTransform, ref List<Weakness> refWeaknesses)
    {
        refTransform.position = position;

        if (health.Count < 1)
        {
            refTransform.gameObject.SetActive(false);
            return;
        }

        var temp = new List<CreateWeaknessEvent>();
        var entity = refTransform.GetComponent<EntityBase>();
        
        for (int i = 0; i < health.Count; i++)
        {
            if (i <= refWeaknesses.Count - 1)
            {
                if (refWeaknesses[i].WeakType != (WeakTypes)health[i].weakType)
                    refWeaknesses[i].SetWeakType((WeakTypes)health[i].weakType);
            }
            else
            {
                temp.Add(new CreateWeaknessEvent(entity, (WeakTypes)health[i].weakType, health[i].iconPosition));
            }
        }
        
        if (refWeaknesses.Count > health.Count)
        {
            for (int i = refWeaknesses.Count - 1; i >= health.Count; i--)
            {
                refWeaknesses[i].SetWeakType(WeakTypes.NONE);
            }
        }
        
        for (int i = 0; i < temp.Count; i++)
        {
            EventBus<CreateWeaknessEvent>.Raise(temp[i]);
        }
    }
}

[Serializable]
public struct PuzzleOutputSaveData
{
    public int id;
    public bool isSolved;

    public PuzzleOutputSaveData(int id, bool isSolved)
    {
        this.id = id;
        this.isSolved = isSolved;
    }

    public void Save(bool isSolved)
    {
        this.isSolved = isSolved;
    }

    public void Load(ref IPuzzleOutput puzzleOutput)
    {
        puzzleOutput.IsSolved = isSolved;
    }

    public int GetID()
    {
        return id;
    }
}

#endregion