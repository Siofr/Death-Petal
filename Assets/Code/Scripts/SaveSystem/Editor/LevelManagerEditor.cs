using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        if (GUILayout.Button("FindSaveables"))
        {
            var instance = (LevelManager)target;
            
            instance.FindSaveables();
        }

        if (GUILayout.Button("Bake Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.BakeLevelData();
        }

        if (GUILayout.Button("Load Default Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.LoadDefaultData();
        }
        
        if (GUILayout.Button("Clear Data"))
        {
            var instance = (LevelManager)target;
            
            instance.ClearData();
        }
    }
}