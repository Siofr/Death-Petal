using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSaveableData_SO))]
public class LevelSaveableData_SOEditor: Editor
{
    private LevelSaveableData_SO data;

    private List<int> id = new List<int>();
    private List<ISaveable> saveables = new List<ISaveable>();
    
    public void OnEnable()
    {
        data =  (LevelSaveableData_SO)target;

        foreach (var elem in data.saveableIDs)
        {
            id.Add(elem.Value);
            saveables.Add(elem.Key);
        }
    }

    public override void OnInspectorGUI()
    {
        for (int i = 0; i < id.Count; i++)
        {
            //EditorGUILayout.LabelField(saveables[i].SaveableName);
            EditorGUILayout.IntField(id[i]);
        }
    }
}