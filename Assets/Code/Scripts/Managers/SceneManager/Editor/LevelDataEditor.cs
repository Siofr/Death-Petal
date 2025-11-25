using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Find Saveables"))
        {
            LevelData levelData = (LevelData)target;
            
            levelData.FindSaveables();
        }
        
        if (GUILayout.Button("Bake Save Data"))
        {
            var levelData = (LevelData) target;
            levelData.BakeLevelData();
        }
        
        if (GUILayout.Button("Load Default Save Data"))
        {
            var levelData = (LevelData) target;
            levelData.LoadDefaultLevelData();
        }

        // if (GUILayout.Button("Clear Save Data"))
        // {
        //     var levelData = (LevelData) target;
        // }
    }
}