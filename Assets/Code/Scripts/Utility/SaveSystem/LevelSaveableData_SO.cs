using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data")]
public class LevelSaveableData_SO: ScriptableObject
{
    public Dictionary<ISaveable, int> saveableIDs = new  Dictionary<ISaveable, int>();
}