using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        
        if (GUILayout.Button("Bake Save Data"))
        {
            LevelData levelData = (LevelData) target;
            
            levelData.SaveLevelData(ref levelData.defaultSaveables);    
        }
        
        if (GUILayout.Button("Load Default Save Data"))
        {
            LevelData levelData = (LevelData) target;
            
            levelData.LoadLevelData(ref levelData.defaultSaveables);    
        }

        if (GUILayout.Button("Clear Save Data"))
        {
            LevelData levelData = (LevelData) target;

            levelData.ClearLevelData();
        }
    }
}