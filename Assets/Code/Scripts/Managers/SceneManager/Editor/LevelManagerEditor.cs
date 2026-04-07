using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var manager = (LevelManager)target;
        
        if (GUILayout.Button("Save Level Data"))
        {
            manager.SaveLevelData();
            EditorUtility.SetDirty(manager);
            EditorUtility.SetDirty(manager.saveableData);
            //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Load Default Level Data"))
        {
            manager.LoadLevelData(true);
        }
        
        if (GUILayout.Button("Load Level Data"))
        {
            manager.LoadLevelData();
            EditorUtility.SetDirty(manager);
            EditorUtility.SetDirty(manager.saveableData);
            //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Clear Level Data"))
        {
            manager.ClearData();
            EditorUtility.SetDirty(manager);
            EditorUtility.SetDirty(manager.saveableData);
            //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}