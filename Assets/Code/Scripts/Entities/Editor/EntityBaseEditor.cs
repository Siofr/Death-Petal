using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityBase), true)][CanEditMultipleObjects]
public class EntityBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Initialise Weaknesses"))
        {
            EntityBase entityBase = (EntityBase) target;
            
            entityBase.InitialiseWeaknesses();
        }

        if (GUILayout.Button("Test OnShot Success"))
        {
            EntityBase entityBase = (EntityBase) target;
            
            entityBase.OnShot(entityBase.Weaknesses[0], entityBase.Weaknesses[0].WeakType);
        }
    }
}