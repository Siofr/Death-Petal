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
        File.WriteAllText(GetSaveDataPath(), JsonUtility.ToJson(gameSaveData, true));
        Debug.Log("Saved Game Data");
        EventBus<LevelSaveCompleteEvent>.Raise( new LevelSaveCompleteEvent());
    }

    public static GameSaveData LoadGameData()
    {
        Debug.Log("Read Game Save Data");
        return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(GetSaveDataPath()));
    }

    public static void ClearData()
    {
        gameSaveData = new GameSaveData(true);
        File.WriteAllText(GetSaveDataPath(), JsonUtility.ToJson(gameSaveData, true));
    }

    public static bool CheckData()
    {
        return gameSaveData.levelSaveData.Count > 0;
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

    public GameSaveData(bool initialise)
    {
        levelSaveData = new List<LevelSaveData>();
    }
    
    static public GameSaveData Initialise()
    {
        var result = new GameSaveData(true);

        if (Directory.Exists(SaveSystem.GetSaveDataPath())  || File.Exists(SaveSystem.GetSaveDataPath()))
        {
            Debug.Log("Found Save Data File");
            result = SaveSystem.LoadGameData();
        }
        else
        {
            Debug.Log("No Save Data File Found");
            File.WriteAllText(SaveSystem.GetSaveDataPath(), JsonUtility.ToJson(result, true));
        }
        
        
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
    
    public GradeSaveData gradeSaveData;
    
    public LevelSaveData(string levelName)
    {
        this.levelName = levelName;
        saveableID = new List<int>();
        entitySaveData = new List<EntitySaveData>();
        puzzleOutputSaveData = new List<PuzzleOutputSaveData>();
        gradeSaveData = new GradeSaveData();
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

[Serializable]
public struct GradeSaveData
{
    public int id;

    public float timeElapsed;
    public int score;
    public int enemyCount;
    
    public string stage;

    public GradeSaveData(int id, float timeElapsed, int score, int enemyCount, string stage)
    {
        this.id = id;
        this.timeElapsed =  timeElapsed;
        this.score = score;
        this.enemyCount = enemyCount;
        this.stage = stage;
    }

    public void Save(GradeManager gradeManager)
    {
        timeElapsed = gradeManager.currentTime;
        score = (int)gradeManager.currentScore;
        enemyCount = gradeManager.enemyCount;
        
        if (gradeManager.currentStage != null)
        {
            stage = gradeManager.currentStage.stageName;
            return;
        }

        stage = "N/A";
    }
    
    public void Load(GradeManager gradeManager)
    {
        gradeManager.currentTime = timeElapsed;
        gradeManager.currentScore = score;
        gradeManager.enemyCount = enemyCount;
        
        if(stage == "N/A") gradeManager.currentStage = null;
        
        var tempStages = GameObject.FindObjectsByType<Stage>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (tempStages.Length < 1) return;
        
        foreach (var tempStage in tempStages)
        {
            if (tempStage.stageName != stage) continue;
            
            gradeManager.currentStage = tempStage;
        }
        
        if(gradeManager.currentStage != null)
        
        EventBus<OnLevelStartEvent>.Raise(new OnLevelStartEvent(gradeManager.currentStage, timeElapsed));
        EventBus<UpdateScoreEvent>.Raise(new UpdateScoreEvent(score, true));
    }
}

#endregion