using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        if (GUILayout.Button("Bake Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.SaveLevelData(true);
        }

        if (GUILayout.Button("Load Default Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.LoadLevelData(true);
        }

        if (GUILayout.Button("Save Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.SaveLevelData(false);
        }

        if (GUILayout.Button("Load Level Data"))
        {
            var instance = (LevelManager)target;
            
            instance.LoadLevelData(false);
        }
        
        if (GUILayout.Button("Clear Data"))
        {
            var instance = (LevelManager)target;
            
            instance.ClearData();
        }
    }
}