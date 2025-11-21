using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Bake Save Data"))
        {
            DrawDefaultInspector();
            
            LevelData levelData = (LevelData) target;
            
            levelData.SaveLevelData();
        }
    }
}