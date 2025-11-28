using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaknessFactory))]
public class WeaknessFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var factory = (WeaknessFactory)target;

        
        if (GUILayout.Button("Create Player Weakness"))
        {
            factory.CreatePlayerWeakness();
        }
    }
}