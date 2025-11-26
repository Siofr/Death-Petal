using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    public static GameSaveData gameSaveData = new GameSaveData();
    
    public static string GetSaveDataPath()
    {
        return Application.persistentDataPath + "/Save.dps";
    }

    public static void SaveData<T>(T Data)
    {
    }

    public static void LoadData<T>()
    {
    }
}

#region Save Data Types
[Serializable]
public struct GameSaveData
{
    public List<LevelSaveData> levelSaveData;
}

[Serializable]
public struct LevelSaveData
{
    public string levelName;
    public List<uint> saveableIDs;
    
    public List<EntitySaveData> entitySaveData;
    public List<PuzzleOutputSaveData> puzzleOutputSaveData;
    public List<PuzzleElementSaveData> puzzleElementSaveData;
}

[Serializable]
public struct EntitySaveData
{
    public Vector3 position;
    public List<int> health;

    public void Save(Vector3 pos, List<Weakness> refWeaknesses)
    {
        position = pos;

        var tempWeaknesses = new List<int>();
        refWeaknesses.ForEach((x)=> tempWeaknesses.Add((int)x.WeakType));

        health = tempWeaknesses;
    }

    public void Load(Transform refTransform, ref List<Weakness> refWeaknesses)
    {
        if (refWeaknesses.Count > health.Count)
        {
            Debug.LogError("Fragmented Health SaveData");
            return;
        }
        
        refTransform.position = position;
        
        for (var i = 0; i < refWeaknesses.Count; i++)
        {
            refWeaknesses[i].SetWeaknessType((WeakTypes)health[i]);
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