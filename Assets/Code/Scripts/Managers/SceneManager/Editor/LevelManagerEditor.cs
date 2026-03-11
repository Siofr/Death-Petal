using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor: Editor
{
    public override void OnInspectorGUI()
    {
        var manager = (LevelManager)target;

        if (GUILayout.Button("Bake Level Data"))
        {
            manager.SaveLevelData(true);
        }
        
        if (GUILayout.Button("Save Level Data"))
        {
            manager.SaveLevelData();
        }

        if (GUILayout.Button("Load Default Level Data"))
        {
            manager.LoadLevelData(true);
        }
        
        if (GUILayout.Button("Load Level Data"))
        {
            manager.LoadLevelData();
        }

        if (GUILayout.Button("Clear Level Data"))
        {
            manager.ClearData();
        }
    }
}