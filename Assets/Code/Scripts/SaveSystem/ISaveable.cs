using System;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO.Hashing;

public interface ISaveable<T>: ISaveable where T : struct
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public int SaveID { get; }
    public void CreateSaveInstance();
    public void DeleteSaveInstance();
    public void HandleSaveData(ref LevelSaveData refData);
    public void HandleLoadData(ref LevelSaveData refData);
}

public class ISaveableHelper
{
    private static HashSet<int> _existingIDs = new HashSet<int>();
    
    public static int GenerateISaveableID()
    {
        var tempID = new Hash128().GetHashCode();

        while (!_existingIDs.Contains(tempID))
        {
            tempID = new Hash128().GetHashCode();
            _existingIDs.Add(tempID);
        }
        
        Debug.Log("New Saveable ID: " + tempID);
        Debug.Log("Existing ID Count: " + _existingIDs.Count);
        
        return tempID;
    }

    public static void AddExistingID(int id)
    {
        _existingIDs.Add(id);
    }
    
    public static void RemoveExistingID(int id)
    {
        _existingIDs.Remove(id);
    }

    public static void ClearAllIDs()
    {
        _existingIDs.Clear();
    }
}
