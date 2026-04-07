using System;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO.Hashing;
using System.Runtime.Serialization;
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
    public static int GenerateISaveableID(LevelSaveableData_SO levelSaveableData)
    {
        var tempID = 1;

        while (levelSaveableData.saveableIDs.Contains(tempID))
        {
            tempID++;
        }
        
        levelSaveableData.saveableIDs.Add(tempID);
        
        Debug.Log(levelSaveableData.saveableIDs.Count);
        Debug.Log("Existing ID Count: " + levelSaveableData.saveableIDs.Count);
        
        return tempID;
    }

    public static Dictionary<string, int> existingNames = new Dictionary<string, int>();
    
    public static void RemoveExistingID(LevelSaveableData_SO levelSaveableData, ISaveable saveable)
    {
        levelSaveableData.saveableIDs.Remove(saveable.SaveID);
    }
    
    public static void RemoveAllIDs(LevelSaveableData_SO levelSaveableData)
    {
        levelSaveableData.saveableIDs.Clear();
    }
}