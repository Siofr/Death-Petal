using System;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO.Hashing;
using Random = UnityEngine.Random;


public interface ISaveable<T> : ISaveable where T : struct
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
        var tempID = Random.Range(0, int.MaxValue);

        while (!_existingIDs.Contains(tempID))
        {
            tempID = Random.Range(0, int.MaxValue);
            _existingIDs.Add(tempID);
        }
        
        Debug.Log("Existing ID Count: " + _existingIDs.Count);
        
        return tempID;
    }
    
    public static void RemoveExistingID(ref int id)
    {
        _existingIDs.Remove(id);
        id = 0;
    }
}
