using System;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO.Hashing;
using UnityEditor;
using Random = UnityEngine.Random;


public interface ISaveable<T>: ISaveable where T : struct
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public string SaveableName { get; }
    public SaveID_SO SaveSO { get; }
    public int SaveID { get; }
    public void CreateSaveInstance(LevelSaveableData_SO levelSaveableData);
    public void DeleteSaveInstance(LevelSaveableData_SO levelSaveableData);
    public void HandleSaveData(ref LevelSaveData refData);
    public void HandleLoadData(ref LevelSaveData refData);
}

public class ISaveableHelper
{
    public static int GenerateISaveableID(LevelSaveableData_SO levelSaveableData, ISaveable saveable)
    {
        var tempID = Random.Range(0, int.MaxValue);

        while (levelSaveableData.saveableIDs.ContainsValue(tempID))
        {
            tempID = Random.Range(0, int.MaxValue);
        }
        
        levelSaveableData.saveableIDs.Add(saveable, tempID);
        
        Debug.Log(levelSaveableData.saveableIDs.Count);
        Debug.Log("Existing ID Count: " + levelSaveableData.saveableIDs.Count);
        
        return tempID;
    }

    public static void RemoveExistingID(LevelSaveableData_SO levelSaveableData, ISaveable saveable)
    {
        levelSaveableData.saveableIDs.Remove(saveable);
    }
    
    public static void RemoveAllIDs(LevelSaveableData_SO levelSaveableData)
    {
        levelSaveableData.saveableIDs.Clear();
    }
}