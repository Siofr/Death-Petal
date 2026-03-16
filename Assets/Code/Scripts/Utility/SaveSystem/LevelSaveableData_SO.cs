using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data")]
public class LevelSaveableData_SO: ScriptableObject
{
    [SerializeField] public List<int> saveableIDs;
}