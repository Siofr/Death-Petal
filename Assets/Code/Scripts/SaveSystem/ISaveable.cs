using UnityEngine;
using System.Collections.Generic;
using System.IO.Hashing;

public interface ISaveable<T>: ISaveable where T : struct
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public int SaveID { get; }
    public void HandleSaveData();
    public void HandleLoadData();
}

public class ISaveableHelper
{
    private static HashSet<int> _existingIDs;
    
    public static int GenerateISaveableID()
    {
        var tempID = new Hash128().GetHashCode();

        while (!_existingIDs.Contains(tempID))
        {
            tempID = new Hash128().GetHashCode();
        }

        return tempID;
    }
}
